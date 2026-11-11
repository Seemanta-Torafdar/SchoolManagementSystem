using SchoolManagement.BLL.DTOs.Examination;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.BLL.Services;

public class ExaminationService : IExaminationService
{
    private readonly IExaminationRepository _examRepo;
    private readonly IResultRepository _resultRepo;
    private readonly IAccountService _accountService;

    public ExaminationService(IExaminationRepository examRepo, IResultRepository resultRepo, IAccountService accountService)
    {
        _examRepo = examRepo;
        _resultRepo = resultRepo;
        _accountService = accountService;
    }

    public async Task<IReadOnlyList<ExaminationDto>> GetAllAsync(int? sessionId = null)
    {
        var exams = sessionId.HasValue 
            ? await _examRepo.GetExaminationsBySessionAsync(sessionId.Value)
            : await _examRepo.GetAllAsync();

        return exams.Select(e => new ExaminationDto
        {
            Id = e.Id,
            Name = e.Name,
            AcademicSessionId = e.AcademicSessionId,
            SessionName = e.AcademicSession?.SessionName ?? $"Session #{e.AcademicSessionId}",
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsPublished = e.IsPublished,
            Description = e.Description,
            SubjectCount = e.ExamSubjects.Count
        }).ToList();
    }

    public async Task<ExaminationDto?> GetByIdAsync(int id)
    {
        var e = await _examRepo.GetExamWithSubjectsAsync(id);
        if (e == null) return null;

        return new ExaminationDto
        {
            Id = e.Id,
            Name = e.Name,
            AcademicSessionId = e.AcademicSessionId,
            SessionName = e.AcademicSession?.SessionName ?? $"Session #{e.AcademicSessionId}",
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsPublished = e.IsPublished,
            Description = e.Description,
            SubjectCount = e.ExamSubjects.Count
        };
    }

    public async Task<ResultResponse<ExaminationDto>> CreateAsync(CreateExaminationDto dto, string? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ResultResponse<ExaminationDto>.Failure("Examination name is required.");

        var exam = new Examination
        {
            Name = dto.Name.Trim(),
            AcademicSessionId = dto.AcademicSessionId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description?.Trim(),
            IsPublished = false
        };

        var created = await _examRepo.AddAsync(exam);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "Examination", created.Id.ToString(), $"Created examination {created.Name}");

        return ResultResponse<ExaminationDto>.Success(new ExaminationDto
        {
            Id = created.Id,
            Name = created.Name,
            AcademicSessionId = created.AcademicSessionId,
            StartDate = created.StartDate,
            EndDate = created.EndDate,
            Description = created.Description,
            IsPublished = created.IsPublished
        }, "Examination created successfully.");
    }

    public async Task<ResultResponse> UpdateAsync(int id, CreateExaminationDto dto, string? currentUserId = null)
    {
        var exam = await _examRepo.GetByIdAsync(id);
        if (exam == null)
            return ResultResponse.Failure("Examination not found.");

        exam.Name = dto.Name.Trim();
        exam.AcademicSessionId = dto.AcademicSessionId;
        exam.StartDate = dto.StartDate;
        exam.EndDate = dto.EndDate;
        exam.Description = dto.Description?.Trim();

        await _examRepo.UpdateAsync(exam);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "Examination", id.ToString(), $"Updated examination {exam.Name}");

        return ResultResponse.Success("Examination updated successfully.");
    }

    public async Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null)
    {
        var exam = await _examRepo.GetByIdAsync(id);
        if (exam == null)
            return ResultResponse.Failure("Examination not found.");

        await _examRepo.DeleteAsync(exam);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "Examination", id.ToString(), $"Deleted examination {exam.Name}");

        return ResultResponse.Success("Examination deleted successfully.");
    }

    public async Task<ResultResponse> PublishExamResultsAsync(int id, bool publish, string? currentUserId = null)
    {
        var exam = await _examRepo.GetByIdAsync(id);
        if (exam == null)
            return ResultResponse.Failure("Examination not found.");

        await _resultRepo.PublishResultsAsync(id, publish);
        await _accountService.LogAuditAsync(currentUserId, "Admin", publish ? "Publish" : "Unpublish", "ExaminationResults", id.ToString(), $"{(publish ? "Published" : "Unpublished")} results for examination {exam.Name}");

        return ResultResponse.Success($"Results for {exam.Name} are now {(publish ? "published" : "unpublished")}.");
    }

    public async Task<IReadOnlyList<ExamSubjectDto>> GetExamSubjectsAsync(int examinationId)
    {
        var list = await _examRepo.GetExamSubjectsAsync(examinationId);
        return list.Select(es => new ExamSubjectDto
        {
            Id = es.Id,
            ExaminationId = es.ExaminationId,
            SubjectId = es.SubjectId,
            SubjectName = es.Subject?.Name ?? "",
            SubjectCode = es.Subject?.SubjectCode ?? "",
            ClassName = es.Subject?.SchoolClass?.Name ?? "",
            ExamDate = es.ExamDate,
            FullMarks = es.FullMarks,
            PassMarks = es.PassMarks
        }).ToList();
    }

    public async Task<ResultResponse<ExamSubjectDto>> AddExamSubjectAsync(CreateExamSubjectDto dto, string? currentUserId = null)
    {
        var examSubject = new ExamSubject
        {
            ExaminationId = dto.ExaminationId,
            SubjectId = dto.SubjectId,
            ExamDate = dto.ExamDate,
            FullMarks = dto.FullMarks,
            PassMarks = dto.PassMarks
        };

        var created = await _examRepo.AddExamSubjectAsync(examSubject);
        return ResultResponse<ExamSubjectDto>.Success(new ExamSubjectDto
        {
            Id = created.Id,
            ExaminationId = created.ExaminationId,
            SubjectId = created.SubjectId,
            ExamDate = created.ExamDate,
            FullMarks = created.FullMarks,
            PassMarks = created.PassMarks
        }, "Exam subject configured successfully.");
    }

    public async Task<ResultResponse> DeleteExamSubjectAsync(int examSubjectId, string? currentUserId = null)
    {
        await _examRepo.DeleteExamSubjectAsync(examSubjectId);
        return ResultResponse.Success("Exam subject removed successfully.");
    }
}

