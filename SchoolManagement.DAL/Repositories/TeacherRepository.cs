using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Teacher?> GetTeacherWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(t => t.User)
            .Include(t => t.Subjects)
                .ThenInclude(sub => sub.SchoolClass)
            .Include(t => t.Routines)
                .ThenInclude(r => r.SchoolClass)
            .Include(t => t.Routines)
                .ThenInclude(r => r.Section)
            .Include(t => t.Routines)
                .ThenInclude(r => r.Subject)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Teacher?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(t => t.User)
            .Include(t => t.Subjects)
                .ThenInclude(sub => sub.SchoolClass)
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<Teacher?> GetByTeacherIdNumberAsync(string teacherIdNumber)
    {
        return await _dbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TeacherIdNumber == teacherIdNumber);
    }

    public async Task<IReadOnlyList<Teacher>> GetPublicTeachersAsync()
    {
        return await _dbSet
            .Include(t => t.Subjects)
            .Where(t => t.IsActive && t.IsPubliclyVisible)
            .OrderBy(t => t.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Teacher>> SearchTeachersAsync(string? searchTerm, int? subjectId, bool? isActive, int page, int pageSize)
    {
        var query = _dbSet
            .Include(t => t.Subjects)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(t => t.FirstName.ToLower().Contains(term) 
                                  || t.LastName.ToLower().Contains(term) 
                                  || t.Designation.ToLower().Contains(term)
                                  || t.TeacherIdNumber.ToLower().Contains(term));
        }

        if (subjectId.HasValue && subjectId > 0)
            query = query.Where(t => t.Subjects.Any(s => s.Id == subjectId.Value));

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        return await query
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm, int? subjectId, bool? isActive)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(t => t.FirstName.ToLower().Contains(term) 
                                  || t.LastName.ToLower().Contains(term) 
                                  || t.Designation.ToLower().Contains(term)
                                  || t.TeacherIdNumber.ToLower().Contains(term));
        }

        if (subjectId.HasValue && subjectId > 0)
            query = query.Where(t => t.Subjects.Any(s => s.Id == subjectId.Value));

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        return await query.CountAsync();
    }
}

