using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Student : AuditableEntity
{
    public string AdmissionNumber { get; set; } = string.Empty; // e.g. "STD-2026-001"
    public int RollNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "Male";
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string GuardianPhone { get; set; } = string.Empty;
    public string? GuardianEmail { get; set; }
    public string? PhotoUrl { get; set; }

    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int SectionId { get; set; }
    public Section? Section { get; set; }

    public int AcademicSessionId { get; set; }
    public AcademicSession? AcademicSession { get; set; }

    public StudentStatus Status { get; set; } = StudentStatus.Active;

    // Secure verification code for result check without login (Part 8 requirement)
    public string VerificationCode { get; set; } = string.Empty;

    public ICollection<StudentResult> Results { get; set; } = new List<StudentResult>();
}

