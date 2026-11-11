using SchoolManagement.BLL.DTOs.Teacher;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface ITeacherService
{
    Task<TeacherDto?> GetByIdAsync(int id);
    Task<TeacherDto?> GetByUserIdAsync(string userId);
    Task<IReadOnlyList<TeacherDto>> GetPublicTeachersAsync();
    Task<PaginatedList<TeacherDto>> SearchTeachersAsync(string? searchTerm, int? subjectId, bool? isActive, int page, int pageSize);
    Task<ResultResponse<TeacherDto>> CreateAsync(CreateTeacherDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(UpdateTeacherDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateSelfProfileAsync(TeacherSelfProfileUpdateDto dto, string userId);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
    Task<ResultResponse> ToggleStatusAsync(int id, string? currentUserId = null);
    Task<int> GetTotalCountAsync();
}

