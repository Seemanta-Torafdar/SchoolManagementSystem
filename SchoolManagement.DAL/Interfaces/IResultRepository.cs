using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface IResultRepository : IGenericRepository<StudentResult>
{
    Task<IReadOnlyList<StudentResult>> GetStudentResultsAsync(int studentId, int examinationId);
    Task<IReadOnlyList<StudentResult>> GetResultsByExamAndClassAsync(int examinationId, int classId, int? sectionId);
    Task<bool> HasResultsForExamSubjectAsync(int examinationId, int subjectId);
    Task SaveBulkResultsAsync(IEnumerable<StudentResult> results);
    Task PublishResultsAsync(int examinationId, bool publish);
}

