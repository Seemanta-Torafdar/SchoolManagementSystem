using SchoolManagement.BLL.DTOs.Student;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IAccountService _accountService;

    public StudentService(IStudentRepository studentRepo, IAccountService accountService)
    {
        _studentRepo = studentRepo;
        _accountService = accountService;
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var s = await _studentRepo.GetStudentWithDetailsAsync(id);
        return s == null ? null : MapToDto(s);
    }

    public async Task<StudentDto?> GetByAdmissionNumberAsync(string admissionNumber)
    {
        var s = await _studentRepo.GetByAdmissionNumberAsync(admissionNumber);
        return s == null ? null : MapToDto(s);
    }

    public async Task<PaginatedList<StudentDto>> SearchStudentsAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId, int page, int pageSize)
    {
        var items = await _studentRepo.SearchStudentsAsync(searchTerm, classId, sectionId, sessionId, page, pageSize);
        var total = await _studentRepo.GetTotalCountAsync(searchTerm, classId, sectionId, sessionId);
        var dtos = items.Select(MapToDto).ToList();
        return new PaginatedList<StudentDto>(dtos, total, page, pageSize);
    }

    public async Task<IReadOnlyList<StudentDto>> GetStudentsByClassAndSectionAsync(int classId, int sectionId, int sessionId)
    {
        var items = await _studentRepo.GetStudentsByClassAndSectionAsync(classId, sectionId, sessionId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<ResultResponse<StudentDto>> CreateAsync(CreateStudentDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return ResultResponse<StudentDto>.Failure("First and Last name are required.");

        var existing = await _studentRepo.GetByAdmissionNumberAsync(dto.AdmissionNumber);
        if (existing != null)
            return ResultResponse<StudentDto>.Failure("Admission number already exists.");

        // Verification code generation if not provided
        var verificationCode = string.IsNullOrWhiteSpace(dto.VerificationCode)
            ? new Random().Next(100000, 999999).ToString()
            : dto.VerificationCode.Trim();

        var student = new Student
        {
            AdmissionNumber = dto.AdmissionNumber.Trim(),
            RollNumber = dto.RollNumber,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            FatherName = dto.FatherName?.Trim(),
            MotherName = dto.MotherName?.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            BloodGroup = dto.BloodGroup,
            Address = dto.Address?.Trim(),
            GuardianPhone = dto.GuardianPhone.Trim(),
            GuardianEmail = dto.GuardianEmail?.Trim(),
            PhotoUrl = dto.PhotoUrl,
            SchoolClassId = dto.SchoolClassId,
            SectionId = dto.SectionId,
            AcademicSessionId = dto.AcademicSessionId,
            Status = dto.Status,
            VerificationCode = verificationCode
        };

        var created = await _studentRepo.AddAsync(student);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Student", created.Id.ToString(), $"Added student {created.FullName} (Roll {created.RollNumber})");

        var detailed = await _studentRepo.GetStudentWithDetailsAsync(created.Id);
        return ResultResponse<StudentDto>.Success(MapToDto(detailed ?? created), "Student created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(UpdateStudentDto dto, string? currentUserId = null)
    {
        var student = await _studentRepo.GetByIdAsync(dto.Id);
        if (student == null)
            return ResultResponse.Failure("Student not found.");

        student.RollNumber = dto.RollNumber;
        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.FatherName = dto.FatherName?.Trim();
        student.MotherName = dto.MotherName?.Trim();
        student.DateOfBirth = dto.DateOfBirth;
        student.Gender = dto.Gender;
        student.BloodGroup = dto.BloodGroup;
        student.Address = dto.Address?.Trim();
        student.GuardianPhone = dto.GuardianPhone.Trim();
        student.GuardianEmail = dto.GuardianEmail?.Trim();
        if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
            student.PhotoUrl = dto.PhotoUrl;
        student.SchoolClassId = dto.SchoolClassId;
        student.SectionId = dto.SectionId;
        student.AcademicSessionId = dto.AcademicSessionId;
        student.Status = dto.Status;
        if (!string.IsNullOrWhiteSpace(dto.VerificationCode))
            student.VerificationCode = dto.VerificationCode.Trim();

        await _studentRepo.UpdateAsync(student);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Student", student.Id.ToString(), $"Updated student {student.FullName}");

        return ResultResponse.Success("Student updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var student = await _studentRepo.GetByIdAsync(id);
        if (student == null)
            return ResultResponse.Failure("Student not found.");

        await _studentRepo.DeleteAsync(student);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Student", id.ToString(), $"Soft-deleted student {student.FullName}");

        return ResultResponse.Success("Student deactivated successfully.");
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _studentRepo.CountAsync();
    }

    private static StudentDto MapToDto(Student s)
    {
        return new StudentDto
        {
            Id = s.Id,
            AdmissionNumber = s.AdmissionNumber,
            RollNumber = s.RollNumber,
            FirstName = s.FirstName,
            LastName = s.LastName,
            FullName = s.FullName,
            FatherName = s.FatherName,
            MotherName = s.MotherName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            BloodGroup = s.BloodGroup,
            Address = s.Address,
            GuardianPhone = s.GuardianPhone,
            GuardianEmail = s.GuardianEmail,
            PhotoUrl = s.PhotoUrl,
            SchoolClassId = s.SchoolClassId,
            ClassName = s.SchoolClass?.Name ?? $"Class #{s.SchoolClassId}",
            SectionId = s.SectionId,
            SectionName = s.Section?.Name ?? $"Section #{s.SectionId}",
            AcademicSessionId = s.AcademicSessionId,
            SessionName = s.AcademicSession?.SessionName ?? $"Session #{s.AcademicSessionId}",
            Status = s.Status,
            VerificationCode = s.VerificationCode
        };
    }
}

