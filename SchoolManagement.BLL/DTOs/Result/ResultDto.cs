using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.DTOs.Result;

public class SubjectResultDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal MaxMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public ResultStatus Status { get; set; }
    public string? Remarks { get; set; }
}

public class StudentMarksheetDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNumber { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string SessionName { get; set; } = string.Empty;
    public string ExaminationName { get; set; } = string.Empty;
    public DateTime ExaminationDate { get; set; }
    public string? PhotoUrl { get; set; }

    public List<SubjectResultDto> Subjects { get; set; } = new();

    public decimal TotalMarksObtained { get; set; }
    public decimal TotalMaxMarks { get; set; }
    public decimal GPA { get; set; }
    public string OverallGrade { get; set; } = string.Empty;
    public ResultStatus ResultStatus { get; set; }
    public bool IsPublished { get; set; }
}

public class BulkMarksEntryItemDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal MarksObtained { get; set; }
    public string? Remarks { get; set; }
}

public class BulkMarksEntryDto
{
    public int ExaminationId { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int SubjectId { get; set; }
    public decimal MaxMarks { get; set; } = 100;
    public List<BulkMarksEntryItemDto> MarksEntries { get; set; } = new();
}

public class ResultSearchQueryDto
{
    public int ExaminationId { get; set; }
    public int AcademicSessionId { get; set; }
    public int ClassId { get; set; }
    public int RollNumber { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
}

