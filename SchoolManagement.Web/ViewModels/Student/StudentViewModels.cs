using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Class;
using SchoolManagement.BLL.DTOs.Student;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Web.ViewModels.Student;

public class StudentListViewModel
{
    public PaginatedList<StudentDto> Students { get; set; } = null!;
    public string? SearchTerm { get; set; }
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? SessionId { get; set; }

    public SelectList? Classes { get; set; }
    public SelectList? Sections { get; set; }
    public SelectList? Sessions { get; set; }
}

public class StudentCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Admission number is required")]
    [Display(Name = "Admission Number")]
    public string AdmissionNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Roll number is required")]
    [Range(1, 9999, ErrorMessage = "Roll number must be between 1 and 9999")]
    [Display(Name = "Roll Number")]
    public int RollNumber { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Father's Name")]
    public string? FatherName { get; set; }

    [Display(Name = "Mother's Name")]
    public string? MotherName { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-14);

    [Required]
    [Display(Name = "Gender")]
    public string Gender { get; set; } = "Male";

    [Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    [Display(Name = "Residential Address")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Guardian phone number is required")]
    [Phone]
    [Display(Name = "Guardian Phone")]
    public string GuardianPhone { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Guardian Email")]
    public string? GuardianEmail { get; set; }

    [Required(ErrorMessage = "Please select a Class")]
    [Display(Name = "Class")]
    public int SchoolClassId { get; set; }

    [Required(ErrorMessage = "Please select a Section")]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Please select an Academic Session")]
    [Display(Name = "Academic Session")]
    public int AcademicSessionId { get; set; }

    [Display(Name = "Status")]
    public StudentStatus Status { get; set; } = StudentStatus.Active;

    [Display(Name = "Private Verification PIN (Part 8 Result Check)")]
    public string? VerificationCode { get; set; }

    public IFormFile? PhotoFile { get; set; }
    public string? ExistingPhotoUrl { get; set; }

    public SelectList? Classes { get; set; }
    public SelectList? Sections { get; set; }
    public SelectList? Sessions { get; set; }
}

