using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Teacher : AuditableEntity
{
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string TeacherIdNumber { get; set; } = string.Empty; // e.g. "TCH-1001"
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Designation { get; set; } = string.Empty; // e.g. "Senior Teacher", "Assistant Teacher"
    public string Qualification { get; set; } = string.Empty; // e.g. "M.Sc in Mathematics, B.Ed"
    public int ExperienceYears { get; set; } = 0;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public string? ProfilePictureUrl { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPubliclyVisible { get; set; } = true;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ClassRoutine> Routines { get; set; } = new List<ClassRoutine>();
    public ICollection<ProfileUpdateRequest> ProfileRequests { get; set; } = new List<ProfileUpdateRequest>();
}

