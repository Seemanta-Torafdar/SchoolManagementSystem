using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subject?> GetSubjectWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsByClassIdAsync(int classId)
    {
        return await _dbSet
            .Include(s => s.Teacher)
            .Where(s => s.SchoolClassId == classId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsByTeacherIdAsync(int teacherId)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Where(s => s.TeacherId == teacherId)
            .OrderBy(s => s.SchoolClass!.NumericValue)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }
}

