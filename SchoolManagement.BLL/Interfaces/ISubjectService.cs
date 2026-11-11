using SchoolManagement.BLL.DTOs.Subject;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface ISubjectService
{
    Task<IReadOnlyList<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<SubjectDto>> GetByClassIdAsync(int classId);
    Task<IReadOnlyList<SubjectDto>> GetByTeacherIdAsync(int teacherId);
    Task<ResultResponse<SubjectDto>> CreateAsync(CreateSubjectDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(int id, CreateSubjectDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
}

