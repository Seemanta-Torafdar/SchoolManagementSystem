namespace SchoolManagement.BLL.DTOs.Subject;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal FullMarks => TotalMarks;
    public decimal PassMarks { get; set; }
    public bool IsOptional { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public int SchoolClassId { get; set; }
    public int? TeacherId { get; set; }
}
