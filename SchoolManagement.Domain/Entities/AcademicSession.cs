using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class AcademicSession : AuditableEntity
{
    public string SessionName { get; set; } = string.Empty; // e.g., "2026" or "2026-2027"
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Examination> Examinations { get; set; } = new List<Examination>();
}

