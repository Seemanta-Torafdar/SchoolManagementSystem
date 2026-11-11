using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Examination : AuditableEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "Annual Examination 2026", "Half-Yearly Examination 2026"
    public int AcademicSessionId { get; set; }
    public AcademicSession? AcademicSession { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPublished { get; set; } = false;
    public string? Description { get; set; }

    public ICollection<ExamSubject> ExamSubjects { get; set; } = new List<ExamSubject>();
    public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
}

