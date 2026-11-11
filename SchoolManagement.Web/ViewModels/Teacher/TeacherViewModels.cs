using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Teacher;
using SchoolManagement.BLL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Web.ViewModels.Teacher;

public class TeacherListViewModel
{
    public PaginatedList<TeacherDto> Teachers { get; set; } = null!;
    public string? SearchTerm { get; set; }
    public int? SubjectId { get; set; }
    public bool? IsActive { get; set; }

    public SelectList? Subjects { get; set; }
}

public class TeacherCreateEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Teacher ID")]
    public string? TeacherIdNumber { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required")]
    [Display(Name = "Designation")]
    public string Designation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Qualification is required")]
    [Display(Name = "Academic Qualifications")]
    public string Qualification { get; set; } = string.Empty;

    [Display(Name = "Experience (Years)")]
    [Range(0, 60)]
    public int ExperienceYears { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone]
    [Display(Name = "Mobile Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [Display(Name = "Official Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Gender")]
    public string Gender { get; set; } = "Male";

    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Permanent / Present Address")]
    public string? Address { get; set; }

    [Display(Name = "Biography / Profile Overview")]
    public string? Bio { get; set; }

    [Display(Name = "Active Account")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Publish on Public School Website")]
    public bool IsPubliclyVisible { get; set; } = true;

    [Display(Name = "Initial Temporary Password")]
    public string? TemporaryPassword { get; set; } = "Teacher@123456";

    public IFormFile? PhotoFile { get; set; }
    public string? ExistingPhotoUrl { get; set; }
}

public class TeacherSelfProfileViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Display(Name = "Mobile Number")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Contact Address")]
    public string? Address { get; set; }

    [Display(Name = "Bio Summary")]
    public string? Bio { get; set; }

    public string? ExistingPhotoUrl { get; set; }
    public IFormFile? PhotoFile { get; set; }
}

