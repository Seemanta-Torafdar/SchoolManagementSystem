using SchoolManagement.BLL.DTOs.Subject;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepo;
    private readonly IAccountService _accountService;

    public SubjectService(ISubjectRepository subjectRepo, IAccountService accountService)
    {
        _subjectRepo = subjectRepo;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<SubjectDto>> GetAllAsync()
    {
        var items = await _subjectRepo.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var s = await _subjectRepo.GetSubjectWithDetailsAsync(id);
        return s == null ? null : MapToDto(s);
    }

    public async Task<IReadOnlyList<SubjectDto>> GetByClassIdAsync(int classId)
    {
        var items = await _subjectRepo.GetSubjectsByClassIdAsync(classId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SubjectDto>> GetByTeacherIdAsync(int teacherId)
    {
        var items = await _subjectRepo.GetSubjectsByTeacherIdAsync(teacherId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<ResultResponse<SubjectDto>> CreateAsync(CreateSubjectDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.SubjectCode))
            return ResultResponse<SubjectDto>.Failure("Subject name and code are required.");

        var subject = new Subject
        {
            Name = dto.Name.Trim(),
            SubjectCode = dto.SubjectCode.Trim().ToUpper(),
            TotalMarks = dto.TotalMarks,
            PassMarks = dto.PassMarks,
            SchoolClassId = dto.SchoolClassId,
            TeacherId = dto.TeacherId
        };

        var created = await _subjectRepo.AddAsync(subject);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Subject", created.Id.ToString(), $"Created subject {created.Name} ({created.SubjectCode})");

        var detailed = await _subjectRepo.GetSubjectWithDetailsAsync(created.Id);
        return ResultResponse<SubjectDto>.Success(MapToDto(detailed ?? created), "Subject created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(int id, CreateSubjectDto dto, string? currentUserId = null)
    {
        var subject = await _subjectRepo.GetByIdAsync(id);
        if (subject == null)
            return ResultResponse.Failure("Subject not found.");

        subject.Name = dto.Name.Trim();
        subject.SubjectCode = dto.SubjectCode.Trim().ToUpper();
        subject.TotalMarks = dto.TotalMarks;
        subject.PassMarks = dto.PassMarks;
        subject.SchoolClassId = dto.SchoolClassId;
        subject.TeacherId = dto.TeacherId;

        await _subjectRepo.UpdateAsync(subject);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Subject", id.ToString(), $"Updated subject {subject.Name}");

        return ResultResponse.Success("Subject updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var subject = await _subjectRepo.GetByIdAsync(id);
        if (subject == null)
            return ResultResponse.Failure("Subject not found.");

        await _subjectRepo.DeleteAsync(subject);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Subject", id.ToString(), $"Deleted subject {subject.Name}");

        return ResultResponse.Success("Subject deleted successfully.");
    }

    private static SubjectDto MapToDto(Subject s)
    {
        return new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            SubjectCode = s.SubjectCode,
            TotalMarks = s.TotalMarks,
            PassMarks = s.PassMarks,
            SchoolClassId = s.SchoolClassId,
            ClassName = s.SchoolClass?.Name ?? $"Class #{s.SchoolClassId}",
            TeacherId = s.TeacherId,
            TeacherName = s.Teacher?.FullName
        };
    }
}

