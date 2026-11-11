using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface IExaminationRepository : IGenericRepository<Examination>
{
    Task<Examination?> GetExamWithSubjectsAsync(int id);
    Task<IReadOnlyList<Examination>> GetExaminationsBySessionAsync(int sessionId);
    Task<IReadOnlyList<ExamSubject>> GetExamSubjectsAsync(int examinationId);
    Task<ExamSubject> AddExamSubjectAsync(ExamSubject examSubject);
    Task DeleteExamSubjectAsync(int examSubjectId);
}

