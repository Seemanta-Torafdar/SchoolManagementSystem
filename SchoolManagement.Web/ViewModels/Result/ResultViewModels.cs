using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Result;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Web.ViewModels.Result;

public class PublicResultSearchViewModel
{
    [Required(ErrorMessage = "Please select an examination")]
    [Display(Name = "Examination")]
    public int ExaminationId { get; set; }

    [Required(ErrorMessage = "Please select academic session")]
    [Display(Name = "Academic Year / Session")]
    public int AcademicSessionId { get; set; }

    [Required(ErrorMessage = "Please select class")]
    [Display(Name = "Class")]
    public int ClassId { get; set; }

    [Required(ErrorMessage = "Please enter roll number")]
    [Range(1, 9999, ErrorMessage = "Roll number must be between 1 and 9999")]
    [Display(Name = "Roll Number")]
    public int RollNumber { get; set; }

    [Required(ErrorMessage = "Please enter your private verification PIN")]
    [Display(Name = "Verification PIN")]
    public string VerificationCode { get; set; } = string.Empty;

    public SelectList? Examinations { get; set; }
    public SelectList? Sessions { get; set; }
    public SelectList? Classes { get; set; }

    public StudentMarksheetDto? Marksheet { get; set; }
    public string? ErrorMessage { get; set; }
}

public class MarksEntryFilterViewModel
{
    [Required]
    [Display(Name = "Examination")]
    public int ExaminationId { get; set; }

    [Required]
    [Display(Name = "Class")]
    public int ClassId { get; set; }

    [Required]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    public SelectList? Examinations { get; set; }
    public SelectList? Classes { get; set; }
    public SelectList? Sections { get; set; }
    public SelectList? Subjects { get; set; }

    public BulkMarksEntryDto? BulkEntry { get; set; }
}

