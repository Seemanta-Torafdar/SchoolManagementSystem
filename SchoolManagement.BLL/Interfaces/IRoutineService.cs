using SchoolManagement.BLL.DTOs.Routine;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IRoutineService
{
    Task<IReadOnlyList<RoutineDto>> GetRoutineByClassAndSectionAsync(int classId, int sectionId);
    Task<IReadOnlyList<RoutineDto>> GetRoutineByTeacherAsync(int teacherId);
    Task<RoutineDto?> GetByIdAsync(int id);
    Task<ResultResponse<RoutineDto>> CreateAsync(CreateRoutineDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(int id, CreateRoutineDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
}

