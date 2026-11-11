using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Subject : AuditableEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "Mathematics", "English", "Physics"
    public string SubjectCode { get; set; } = string.Empty; // e.g. "MTH-101"
    public decimal TotalMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;

    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public ICollection<ExamSubject> ExamSubjects { get; set; } = new List<ExamSubject>();
    public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
    public ICollection<ClassRoutine> Routines { get; set; } = new List<ClassRoutine>();
}

