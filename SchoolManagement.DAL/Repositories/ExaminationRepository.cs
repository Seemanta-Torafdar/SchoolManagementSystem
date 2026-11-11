using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class ExaminationRepository : GenericRepository<Examination>, IExaminationRepository
{
    public ExaminationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Examination?> GetExamWithSubjectsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.AcademicSession)
            .Include(e => e.ExamSubjects)
                .ThenInclude(es => es.Subject)
                    .ThenInclude(sub => sub!.SchoolClass)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<Examination>> GetExaminationsBySessionAsync(int sessionId)
    {
        return await _dbSet
            .Include(e => e.AcademicSession)
            .Include(e => e.ExamSubjects)
            .Where(e => e.AcademicSessionId == sessionId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ExamSubject>> GetExamSubjectsAsync(int examinationId)
    {
        return await _context.ExamSubjects
            .Include(es => es.Subject)
                .ThenInclude(s => s!.SchoolClass)
            .Where(es => es.ExaminationId == examinationId)
            .OrderBy(es => es.ExamDate)
            .ToListAsync();
    }

    public async Task<ExamSubject> AddExamSubjectAsync(ExamSubject examSubject)
    {
        await _context.ExamSubjects.AddAsync(examSubject);
        await _context.SaveChangesAsync();
        return examSubject;
    }

    public async Task DeleteExamSubjectAsync(int examSubjectId)
    {
        var item = await _context.ExamSubjects.FindAsync(examSubjectId);
        if (item != null)
        {
            _context.ExamSubjects.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}

