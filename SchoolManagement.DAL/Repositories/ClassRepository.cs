using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class ClassRepository : GenericRepository<SchoolClass>, IClassRepository
{
    public ClassRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SchoolClass?> GetClassWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Sections)
            .Include(c => c.Subjects)
                .ThenInclude(sub => sub.Teacher)
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<SchoolClass>> GetAllClassesWithSectionsAsync()
    {
        return await _dbSet
            .Include(c => c.Sections)
            .Include(c => c.Subjects)
            .OrderBy(c => c.NumericValue)
            .ToListAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(int sectionId)
    {
        return await _context.Sections
            .Include(s => s.SchoolClass)
            .FirstOrDefaultAsync(s => s.Id == sectionId);
    }

    public async Task<IReadOnlyList<Section>> GetSectionsByClassIdAsync(int classId)
    {
        return await _context.Sections
            .Where(s => s.SchoolClassId == classId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Section> AddSectionAsync(Section section)
    {
        await _context.Sections.AddAsync(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task UpdateSectionAsync(Section section)
    {
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSectionAsync(Section section)
    {
        section.IsDeleted = true;
        _context.Entry(section).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

