using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class ClassRoutine : AuditableEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int SectionId { get; set; }
    public Section? Section { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public string DayOfWeek { get; set; } = "Sunday"; // Sunday, Monday, Tuesday, Wednesday, Thursday
    public int PeriodNumber { get; set; } = 1; // 1 to 7
    public string StartTime { get; set; } = "09:00 AM";
    public string EndTime { get; set; } = "09:45 AM";
    public string? RoomNumber { get; set; }
}

