namespace SchoolManagement.BLL.DTOs.Examination;

public class ExaminationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AcademicSessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPublished { get; set; }
    public string? Description { get; set; }
    public int SubjectCount { get; set; }
}

public class CreateExaminationDto
{
    public string Name { get; set; } = string.Empty;
    public int AcademicSessionId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(10);
    public string? Description { get; set; }
}

public class ExamSubjectDto
{
    public int Id { get; set; }
    public int ExaminationId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
}

public class CreateExamSubjectDto
{
    public int ExaminationId { get; set; }
    public int SubjectId { get; set; }
    public DateTime ExamDate { get; set; } = DateTime.UtcNow;
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
}

