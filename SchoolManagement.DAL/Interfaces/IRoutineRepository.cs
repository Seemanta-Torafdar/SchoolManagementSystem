using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface IRoutineRepository : IGenericRepository<ClassRoutine>
{
    Task<IReadOnlyList<ClassRoutine>> GetRoutineByClassAndSectionAsync(int classId, int sectionId);
    Task<IReadOnlyList<ClassRoutine>> GetRoutineByTeacherAsync(int teacherId);
    Task<bool> HasConflictAsync(string dayOfWeek, int periodNumber, int? teacherId, int? classId, int? sectionId, int? excludeId = null);
}

