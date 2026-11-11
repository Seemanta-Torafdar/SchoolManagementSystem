namespace SchoolManagement.BLL.DTOs.Routine;

public class RoutineDto
{
    public int Id { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public int PeriodNumber { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string? RoomNumber { get; set; }
}

public class CreateRoutineDto
{
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public string DayOfWeek { get; set; } = "Sunday";
    public int PeriodNumber { get; set; } = 1;
    public string StartTime { get; set; } = "09:00 AM";
    public string EndTime { get; set; } = "09:45 AM";
    public string? RoomNumber { get; set; }
}

