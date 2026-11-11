namespace SchoolManagement.BLL.DTOs.Teacher;

public class TeacherDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string TeacherIdNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public bool IsPubliclyVisible { get; set; }
    public List<string> AssignedSubjects { get; set; } = new();
}

public class CreateTeacherDto
{
    public string TeacherIdNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Gender { get; set; } = "Male";
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public string? ProfilePictureUrl { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPubliclyVisible { get; set; } = true;
    public string? TemporaryPassword { get; set; } = "Teacher@123456";
}

public class UpdateTeacherDto : CreateTeacherDto
{
    public int Id { get; set; }
}

public class TeacherSelfProfileUpdateDto
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

