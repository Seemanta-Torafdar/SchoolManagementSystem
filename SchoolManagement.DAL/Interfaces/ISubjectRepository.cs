using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<Subject?> GetSubjectWithDetailsAsync(int id);
    Task<IReadOnlyList<Subject>> GetSubjectsByClassIdAsync(int classId);
    Task<IReadOnlyList<Subject>> GetSubjectsByTeacherIdAsync(int teacherId);
}

