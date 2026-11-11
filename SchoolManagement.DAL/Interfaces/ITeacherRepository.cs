using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface ITeacherRepository : IGenericRepository<Teacher>
{
    Task<Teacher?> GetTeacherWithDetailsAsync(int id);
    Task<Teacher?> GetByUserIdAsync(string userId);
    Task<Teacher?> GetByTeacherIdNumberAsync(string teacherIdNumber);
    Task<IReadOnlyList<Teacher>> GetPublicTeachersAsync();
    Task<IReadOnlyList<Teacher>> SearchTeachersAsync(string? searchTerm, int? subjectId, bool? isActive, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? searchTerm, int? subjectId, bool? isActive);
}

