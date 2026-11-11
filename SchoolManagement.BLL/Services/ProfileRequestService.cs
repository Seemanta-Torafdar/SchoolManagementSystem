using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Services;

public class ProfileRequestService : IProfileRequestService
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountService _accountService;

    public ProfileRequestService(ApplicationDbContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<ProfileRequestDto>> GetAllRequestsAsync()
    {
        var items = await _context.ProfileUpdateRequests
            .Include(p => p.Teacher)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<ProfileRequestDto>> GetRequestsByTeacherIdAsync(int teacherId)
    {
        var items = await _context.ProfileUpdateRequests
            .Include(p => p.Teacher)
            .Where(p => p.TeacherId == teacherId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<ResultResponse<ProfileRequestDto>> SubmitRequestAsync(CreateProfileRequestDto dto)
    {
        var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
        if (teacher == null)
            return ResultResponse<ProfileRequestDto>.Failure("Teacher not found.");

        var request = new ProfileUpdateRequest
        {
            TeacherId = dto.TeacherId,
            RequestedFirstName = dto.RequestedFirstName.Trim(),
            RequestedLastName = dto.RequestedLastName.Trim(),
            RequestedDesignation = dto.RequestedDesignation.Trim(),
            RequestedQualification = dto.RequestedQualification.Trim(),
            RequestedExperienceYears = dto.RequestedExperienceYears,
            Reason = dto.Reason?.Trim(),
            Status = RequestStatus.Pending
        };

        await _context.ProfileUpdateRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        await _accountService.LogAuditAsync(teacher.UserId, teacher.FullName, "ProfileRequest", "Teacher", teacher.Id.ToString(), "Submitted official profile update request.");

        return ResultResponse<ProfileRequestDto>.Success(MapToDto(request), "Profile update request submitted to Administrator for verification.");
    }

    public async Task<ResultResponse> ReviewRequestAsync(int id, bool approve, string? adminRemarks, string currentUserName)
    {
        var request = await _context.ProfileUpdateRequests
            .Include(p => p.Teacher)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (request == null)
            return ResultResponse.Failure("Request not found.");

        if (request.Status != RequestStatus.Pending)
            return ResultResponse.Failure("This request has already been processed.");

        request.Status = approve ? RequestStatus.Approved : RequestStatus.Rejected;
        request.AdminRemarks = adminRemarks?.Trim();
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedBy = currentUserName;

        if (approve && request.Teacher != null)
        {
            // Apply requested official changes
            request.Teacher.FirstName = request.RequestedFirstName;
            request.Teacher.LastName = request.RequestedLastName;
            request.Teacher.Designation = request.RequestedDesignation;
            request.Teacher.Qualification = request.RequestedQualification;
            request.Teacher.ExperienceYears = request.RequestedExperienceYears;
            _context.Teachers.Update(request.Teacher);
        }

        await _context.SaveChangesAsync();
        await _accountService.LogAuditAsync(null, currentUserName, approve ? "ApproveRequest" : "RejectRequest", "ProfileUpdateRequest", id.ToString(), $"{(approve ? "Approved" : "Rejected")} profile update request for {request.Teacher?.FullName}");

        return ResultResponse.Success($"Request has been {(approve ? "approved and applied" : "rejected")}.");
    }

    private static ProfileRequestDto MapToDto(ProfileUpdateRequest p)
    {
        return new ProfileRequestDto
        {
            Id = p.Id,
            TeacherId = p.TeacherId,
            TeacherName = p.Teacher?.FullName ?? "",
            CurrentDesignation = p.Teacher?.Designation ?? "",
            CurrentQualification = p.Teacher?.Qualification ?? "",
            CurrentExperienceYears = p.Teacher?.ExperienceYears ?? 0,
            RequestedFirstName = p.RequestedFirstName,
            RequestedLastName = p.RequestedLastName,
            RequestedDesignation = p.RequestedDesignation,
            RequestedQualification = p.RequestedQualification,
            RequestedExperienceYears = p.RequestedExperienceYears,
            Reason = p.Reason,
            Status = p.Status,
            AdminRemarks = p.AdminRemarks,
            CreatedAt = p.CreatedAt,
            ReviewedAt = p.ReviewedAt,
            ReviewedBy = p.ReviewedBy
        };
    }
}

