using SchoolManagement.BLL.DTOs.Student;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IStudentService
{
    Task<StudentDto?> GetByIdAsync(int id);
    Task<StudentDto?> GetByAdmissionNumberAsync(string admissionNumber);
    Task<PaginatedList<StudentDto>> SearchStudentsAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId, int page, int pageSize);
    Task<IReadOnlyList<StudentDto>> GetStudentsByClassAndSectionAsync(int classId, int sectionId, int sessionId);
    Task<ResultResponse<StudentDto>> CreateAsync(CreateStudentDto dto, string? currentUserId = null);
    Task<ResultResponse> UpdateAsync(UpdateStudentDto dto, string? currentUserId = null);
    Task<ResultResponse> DeleteAsync(int id, string? currentUserId = null);
    Task<int> GetTotalCountAsync();
}

