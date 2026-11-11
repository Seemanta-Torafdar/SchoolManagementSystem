using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class RoutineRepository : GenericRepository<ClassRoutine>, IRoutineRepository
{
    public RoutineRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ClassRoutine>> GetRoutineByClassAndSectionAsync(int classId, int sectionId)
    {
        return await _dbSet
            .Include(r => r.SchoolClass)
            .Include(r => r.Section)
            .Include(r => r.Subject)
            .Include(r => r.Teacher)
            .Where(r => r.SchoolClassId == classId && r.SectionId == sectionId)
            .OrderBy(r => r.DayOfWeek)
            .ThenBy(r => r.PeriodNumber)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ClassRoutine>> GetRoutineByTeacherAsync(int teacherId)
    {
        return await _dbSet
            .Include(r => r.SchoolClass)
            .Include(r => r.Section)
            .Include(r => r.Subject)
            .Where(r => r.TeacherId == teacherId)
            .OrderBy(r => r.DayOfWeek)
            .ThenBy(r => r.PeriodNumber)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(string dayOfWeek, int periodNumber, int? teacherId, int? classId, int? sectionId, int? excludeId = null)
    {
        var query = _dbSet.Where(r => r.DayOfWeek == dayOfWeek && r.PeriodNumber == periodNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        // Teacher conflict: same teacher cannot teach two classes at the same period
        if (teacherId.HasValue)
        {
            var teacherBusy = await query.AnyAsync(r => r.TeacherId == teacherId.Value);
            if (teacherBusy) return true;
        }

        // Section conflict: same section cannot have two classes at the same period
        if (classId.HasValue && sectionId.HasValue)
        {
            var sectionBusy = await query.AnyAsync(r => r.SchoolClassId == classId.Value && r.SectionId == sectionId.Value);
            if (sectionBusy) return true;
        }

        return false;
    }
}

