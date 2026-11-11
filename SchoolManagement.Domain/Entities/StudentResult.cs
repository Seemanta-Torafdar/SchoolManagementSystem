using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class StudentResult : AuditableEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int ExaminationId { get; set; }
    public Examination? Examination { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public decimal MarksObtained { get; set; }
    public decimal MaxMarks { get; set; } = 100;
    public string Grade { get; set; } = string.Empty; // A+, A, A-, B, C, D, F
    public decimal GradePoint { get; set; } = 0.0m; // 5.0, 4.0, etc.
    public ResultStatus Status { get; set; } = ResultStatus.Passed;
    public string? Remarks { get; set; }
    public bool IsPublished { get; set; } = false;
}

