using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.DTOs.Student;

public class StudentDto
{
    public int Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string GuardianPhone { get; set; } = string.Empty;
    public string? GuardianEmail { get; set; }
    public string? PhotoUrl { get; set; }
    public int SchoolClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int AcademicSessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public StudentStatus Status { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
}

public class CreateStudentDto
{
    public string AdmissionNumber { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-14);
    public string Gender { get; set; } = "Male";
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string GuardianPhone { get; set; } = string.Empty;
    public string? GuardianEmail { get; set; }
    public string? PhotoUrl { get; set; }
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public int AcademicSessionId { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public string? VerificationCode { get; set; }
}

public class UpdateStudentDto : CreateStudentDto
{
    public int Id { get; set; }
}
