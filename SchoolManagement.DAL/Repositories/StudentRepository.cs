using Microsoft.EntityFrameworkCore;
using SchoolManagement.DAL.Data;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetStudentWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Section)
            .Include(s => s.AcademicSession)
            .Include(s => s.Results)
                .ThenInclude(r => r.Subject)
            .Include(s => s.Results)
                .ThenInclude(r => r.Examination)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByAdmissionNumberAsync(string admissionNumber)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Section)
            .Include(s => s.AcademicSession)
            .FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber);
    }

    public async Task<Student?> VerifyStudentForResultAsync(int examinationId, int academicSessionId, int classId, int rollNumber, string verificationCode)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Section)
            .Include(s => s.AcademicSession)
            .Include(s => s.Results.Where(r => r.ExaminationId == examinationId && r.IsPublished))
                .ThenInclude(r => r.Subject)
            .FirstOrDefaultAsync(s => s.AcademicSessionId == academicSessionId 
                                   && s.SchoolClassId == classId 
                                   && s.RollNumber == rollNumber 
                                   && s.VerificationCode.Trim().ToLower() == verificationCode.Trim().ToLower());
    }

    public async Task<IReadOnlyList<Student>> GetStudentsByClassAndSectionAsync(int classId, int sectionId, int academicSessionId)
    {
        return await _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Section)
            .Include(s => s.AcademicSession)
            .Where(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AcademicSessionId == academicSessionId)
            .OrderBy(s => s.RollNumber)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Student>> SearchStudentsAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(s => s.SchoolClass)
            .Include(s => s.Section)
            .Include(s => s.AcademicSession)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(s => s.FirstName.ToLower().Contains(term) 
                                  || s.LastName.ToLower().Contains(term) 
                                  || s.AdmissionNumber.ToLower().Contains(term)
                                  || s.RollNumber.ToString().Contains(term));
        }

        if (classId.HasValue && classId > 0)
            query = query.Where(s => s.SchoolClassId == classId.Value);

        if (sectionId.HasValue && sectionId > 0)
            query = query.Where(s => s.SectionId == sectionId.Value);

        if (sessionId.HasValue && sessionId > 0)
            query = query.Where(s => s.AcademicSessionId == sessionId.Value);

        return await query
            .OrderBy(s => s.SchoolClassId)
            .ThenBy(s => s.SectionId)
            .ThenBy(s => s.RollNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(s => s.FirstName.ToLower().Contains(term) 
                                  || s.LastName.ToLower().Contains(term) 
                                  || s.AdmissionNumber.ToLower().Contains(term)
                                  || s.RollNumber.ToString().Contains(term));
        }

        if (classId.HasValue && classId > 0)
            query = query.Where(s => s.SchoolClassId == classId.Value);

        if (sectionId.HasValue && sectionId > 0)
            query = query.Where(s => s.SectionId == sectionId.Value);

        if (sessionId.HasValue && sessionId > 0)
            query = query.Where(s => s.AcademicSessionId == sessionId.Value);

        return await query.CountAsync();
    }
}

