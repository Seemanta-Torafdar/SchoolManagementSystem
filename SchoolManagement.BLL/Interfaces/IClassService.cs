using SchoolManagement.BLL.DTOs.Class;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IClassService
{
    Task<IReadOnlyList<ClassDto>> GetAllClassesAsync();
    Task<ClassDto?> GetByIdAsync(int id);
    Task<ResultResponse<ClassDto>> CreateClassAsync(CreateClassDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateClassAsync(int id, CreateClassDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteClassAsync(int id, string? currentUserId = null);

    Task<IReadOnlyList<SectionDto>> GetSectionsByClassIdAsync(int classId);
    Task<ResultResponse<SectionDto>> CreateSectionAsync(CreateSectionDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteSectionAsync(int id, string? currentUserId = null);
    Task<int> GetTotalCountAsync();
}

