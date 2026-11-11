using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class ResultRepository : GenericRepository<StudentResult>, IResultRepository
{
    public ResultRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<StudentResult>> GetStudentResultsAsync(int studentId, int examinationId)
    {
        return await _dbSet
            .Include(r => r.Subject)
            .Include(r => r.Examination)
            .Include(r => r.Student)
            .Where(r => r.StudentId == studentId && r.ExaminationId == examinationId)
            .OrderBy(r => r.Subject!.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StudentResult>> GetResultsByExamAndClassAsync(int examinationId, int classId, int? sectionId)
    {
        var query = _dbSet
            .Include(r => r.Student)
                .ThenInclude(s => s!.Section)
            .Include(r => r.Subject)
            .Where(r => r.ExaminationId == examinationId && r.Student!.SchoolClassId == classId);

        if (sectionId.HasValue && sectionId.Value > 0)
        {
            query = query.Where(r => r.Student!.SectionId == sectionId.Value);
        }

        return await query
            .OrderBy(r => r.Student!.RollNumber)
            .ThenBy(r => r.Subject!.Name)
            .ToListAsync();
    }

    public async Task<bool> HasResultsForExamSubjectAsync(int examinationId, int subjectId)
    {
        return await _dbSet.AnyAsync(r => r.ExaminationId == examinationId && r.SubjectId == subjectId);
    }

    public async Task SaveBulkResultsAsync(IEnumerable<StudentResult> results)
    {
        foreach (var result in results)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(r => 
                r.StudentId == result.StudentId && 
                r.ExaminationId == result.ExaminationId && 
                r.SubjectId == result.SubjectId);

            if (existing != null)
            {
                existing.MarksObtained = result.MarksObtained;
                existing.MaxMarks = result.MaxMarks;
                existing.Grade = result.Grade;
                existing.GradePoint = result.GradePoint;
                existing.Status = result.Status;
                existing.Remarks = result.Remarks;
                existing.IsPublished = result.IsPublished;
                _dbSet.Update(existing);
            }
            else
            {
                await _dbSet.AddAsync(result);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task PublishResultsAsync(int examinationId, bool publish)
    {
        var results = await _dbSet.Where(r => r.ExaminationId == examinationId).ToListAsync();
        foreach (var result in results)
        {
            result.IsPublished = publish;
        }

        var exam = await _context.Examinations.FindAsync(examinationId);
        if (exam != null)
        {
            exam.IsPublished = publish;
        }

        await _context.SaveChangesAsync();
    }
}

