using SchoolManagement.BLL.DTOs.Examination;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IExaminationService
{
    Task<IReadOnlyList<ExaminationDto>> GetAllAsync(int? sessionId = null);
    Task<ExaminationDto?> GetByIdAsync(int id);
    Task<ResultResponse<ExaminationDto>> CreateAsync(CreateExaminationDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(int id, CreateExaminationDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
    Task<ResultResponse> PublishExamResultsAsync(int id, bool publish, string? currentUserId = null);

    Task<IReadOnlyList<ExamSubjectDto>> GetExamSubjectsAsync(int examinationId);
    Task<ResultResponse<ExamSubjectDto>> AddExamSubjectAsync(CreateExamSubjectDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteExamSubjectAsync(int examSubjectId, string? currentUserId = null);
}

