using SchoolManagement.Domain.Entities;

namespace SchoolManagement.DAL.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetStudentWithDetailsAsync(int id);
    Task<Student?> GetByAdmissionNumberAsync(string admissionNumber);
    Task<Student?> VerifyStudentForResultAsync(int examinationId, int academicSessionId, int classId, int rollNumber, string verificationCode);
    Task<IReadOnlyList<Student>> GetStudentsByClassAndSectionAsync(int classId, int sectionId, int academicSessionId);
    Task<IReadOnlyList<Student>> SearchStudentsAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? searchTerm, int? classId, int? sectionId, int? sessionId);
}

