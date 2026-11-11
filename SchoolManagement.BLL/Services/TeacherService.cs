using Microsoft.AspNetCore.Identity;
using SchoolManagement.BLL.DTOs.Teacher;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccountService _accountService;

    public TeacherService(
        ITeacherRepository teacherRepo, 
        UserManager<ApplicationUser> userManager,
        IAccountService accountService)
    {
        _teacherRepo = teacherRepo;
        _userManager = userManager;
        _accountService = accountService;
    }

    public async Task<TeacherDto?> GetByIdAsync(int id)
    {
        var t = await _teacherRepo.GetTeacherWithDetailsAsync(id);
        return t == null ? null : MapToDto(t);
    }

    public async Task<TeacherDto?> GetByUserIdAsync(string userId)
    {
        var t = await _teacherRepo.GetByUserIdAsync(userId);
        return t == null ? null : MapToDto(t);
    }

    public async Task<IReadOnlyList<TeacherDto>> GetPublicTeachersAsync()
    {
        var list = await _teacherRepo.GetPublicTeachersAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<PaginatedList<TeacherDto>> SearchTeachersAsync(string? searchTerm, int? subjectId, bool? isActive, int page, int pageSize)
    {
        var items = await _teacherRepo.SearchTeachersAsync(searchTerm, subjectId, isActive, page, pageSize);
        var total = await _teacherRepo.GetTotalCountAsync(searchTerm, subjectId, isActive);
        var dtos = items.Select(MapToDto).ToList();
        return new PaginatedList<TeacherDto>(dtos, total, page, pageSize);
    }

    public async Task<ResultResponse<TeacherDto>> CreateAsync(CreateTeacherDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return ResultResponse<TeacherDto>.Failure("First and Last name are required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return ResultResponse<TeacherDto>.Failure("Email address is required.");

        // Create ApplicationUser for login
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return ResultResponse<TeacherDto>.Failure("A user with this email already exists.");

        var appUser = new ApplicationUser
        {
            UserName = dto.Email.Trim(),
            Email = dto.Email.Trim(),
            EmailConfirmed = true,
            FullName = $"{dto.FirstName.Trim()} {dto.LastName.Trim()}",
            Role = UserRole.Teacher,
            IsActive = dto.IsActive,
            ProfilePictureUrl = dto.ProfilePictureUrl,
            CreatedAt = DateTime.UtcNow
        };

        var password = string.IsNullOrWhiteSpace(dto.TemporaryPassword) ? "Teacher@123456" : dto.TemporaryPassword;
        var identityResult = await _userManager.CreateAsync(appUser, password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            return ResultResponse<TeacherDto>.Failure(errors, "Failed to create teacher login account.");
        }

        await _userManager.AddToRoleAsync(appUser, UserRole.Teacher);

        var teacherIdNumber = string.IsNullOrWhiteSpace(dto.TeacherIdNumber)
            ? $"TCH-{new Random().Next(1000, 9999)}"
            : dto.TeacherIdNumber.Trim();

        var teacher = new Teacher
        {
            UserId = appUser.Id,
            TeacherIdNumber = teacherIdNumber,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Designation = dto.Designation.Trim(),
            Qualification = dto.Qualification.Trim(),
            ExperienceYears = dto.ExperienceYears,
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim(),
            Gender = dto.Gender,
            JoiningDate = dto.JoiningDate,
            ProfilePictureUrl = dto.ProfilePictureUrl,
            Address = dto.Address?.Trim(),
            Bio = dto.Bio?.Trim(),
            IsActive = dto.IsActive,
            IsPubliclyVisible = dto.IsPubliclyVisible
        };

        var created = await _teacherRepo.AddAsync(teacher);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Teacher", created.Id.ToString(), $"Created teacher account for {created.FullName}");

        return ResultResponse<TeacherDto>.Success(MapToDto(created), "Teacher account created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(UpdateTeacherDto dto, string? currentUserId = null)
    {
        var teacher = await _teacherRepo.GetByIdAsync(dto.Id);
        if (teacher == null)
            return ResultResponse.Failure("Teacher not found.");

        teacher.FirstName = dto.FirstName.Trim();
        teacher.LastName = dto.LastName.Trim();
        teacher.Designation = dto.Designation.Trim();
        teacher.Qualification = dto.Qualification.Trim();
        teacher.ExperienceYears = dto.ExperienceYears;
        teacher.Phone = dto.Phone.Trim();
        teacher.Email = dto.Email.Trim();
        teacher.Gender = dto.Gender;
        teacher.JoiningDate = dto.JoiningDate;
        if (!string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
            teacher.ProfilePictureUrl = dto.ProfilePictureUrl;
        teacher.Address = dto.Address?.Trim();
        teacher.Bio = dto.Bio?.Trim();
        teacher.IsActive = dto.IsActive;
        teacher.IsPubliclyVisible = dto.IsPubliclyVisible;

        await _teacherRepo.UpdateAsync(teacher);

        // Update linked user if present
        if (!string.IsNullOrEmpty(teacher.UserId))
        {
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            if (user != null)
            {
                user.FullName = teacher.FullName;
                user.IsActive = teacher.IsActive;
                if (!string.IsNullOrWhiteSpace(teacher.ProfilePictureUrl))
                    user.ProfilePictureUrl = teacher.ProfilePictureUrl;
                await _userManager.UpdateAsync(user);
            }
        }

        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Teacher", teacher.Id.ToString(), $"Updated teacher {teacher.FullName}");
        return ResultResponse.Success("Teacher updated successfully.");
    }

    public async Task<ResultResponse> UpdateSelfProfileAsync(TeacherSelfProfileUpdateDto dto, string userId)
    {
        var teacher = await _teacherRepo.GetByUserIdAsync(userId);
        if (teacher == null)
            return ResultResponse.Failure("Teacher profile not found.");

        teacher.Phone = dto.Phone.Trim();
        teacher.Address = dto.Address?.Trim();
        teacher.Bio = dto.Bio?.Trim();
        if (!string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
            teacher.ProfilePictureUrl = dto.ProfilePictureUrl;

        await _teacherRepo.UpdateAsync(teacher);

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && !string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
        {
            user.ProfilePictureUrl = dto.ProfilePictureUrl;
            await _userManager.UpdateAsync(user);
        }

        await _accountService.LogAuditAsync(userId, teacher.FullName, "Update", "TeacherProfile", teacher.Id.ToString(), "Updated personal contact details.");
        return ResultResponse.Success("Profile updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var teacher = await _teacherRepo.GetByIdAsync(id);
        if (teacher == null)
            return ResultResponse.Failure("Teacher not found.");

        await _teacherRepo.DeleteAsync(teacher);

        if (!string.IsNullOrEmpty(teacher.UserId))
        {
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
            }
        }

        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Teacher", id.ToString(), $"Deactivated teacher {teacher.FullName}");
        return ResultResponse.Success("Teacher deactivated successfully.");
    }

    public async Task<ResultResponse> ToggleStatusAsync(int id, string? currentUserId = null)
    {
        var teacher = await _teacherRepo.GetByIdAsync(id);
        if (teacher == null)
            return ResultResponse.Failure("Teacher not found.");

        teacher.IsActive = !teacher.IsActive;
        await _teacherRepo.UpdateAsync(teacher);

        if (!string.IsNullOrEmpty(teacher.UserId))
        {
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            if (user != null)
            {
                user.IsActive = teacher.IsActive;
                await _userManager.UpdateAsync(user);
            }
        }

        await _accountService.LogAuditAsync(currentUserId, "Admin", "StatusChange", "Teacher", id.ToString(), $"Set status to {(teacher.IsActive ? "Active" : "Inactive")} for {teacher.FullName}");
        return ResultResponse.Success($"Teacher status set to {(teacher.IsActive ? "Active" : "Inactive")}.");
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _teacherRepo.CountAsync();
    }

    private static TeacherDto MapToDto(Teacher t)
    {
        return new TeacherDto
        {
            Id = t.Id,
            UserId = t.UserId,
            TeacherIdNumber = t.TeacherIdNumber,
            FirstName = t.FirstName,
            LastName = t.LastName,
            FullName = t.FullName,
            Designation = t.Designation,
            Qualification = t.Qualification,
            ExperienceYears = t.ExperienceYears,
            Phone = t.Phone,
            Email = t.Email,
            Gender = t.Gender,
            JoiningDate = t.JoiningDate,
            ProfilePictureUrl = t.ProfilePictureUrl,
            Address = t.Address,
            Bio = t.Bio,
            IsActive = t.IsActive,
            IsPubliclyVisible = t.IsPubliclyVisible,
            AssignedSubjects = t.Subjects?.Select(s => s.Name).ToList() ?? new List<string>()
        };
    }
}

