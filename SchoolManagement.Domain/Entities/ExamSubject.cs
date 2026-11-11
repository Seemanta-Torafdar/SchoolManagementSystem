using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class ExamSubject : AuditableEntity
{
    public int ExaminationId { get; set; }
    public Examination? Examination { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public DateTime ExamDate { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
}

