using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Routine;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Web.ViewModels.Routine;

public class RoutineListViewModel
{
    public int? ClassId { get; set; }
    public int? SectionId { get; set; }
    public SelectList? Classes { get; set; }
    public SelectList? Sections { get; set; }
    public IReadOnlyList<RoutineDto> Routines { get; set; } = new List<RoutineDto>();
}

public class RoutineCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a Class")]
    [Display(Name = "Class")]
    public int SchoolClassId { get; set; }

    [Required(ErrorMessage = "Please select a Section")]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Please select a Subject")]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Please select a Teacher")]
    [Display(Name = "Assigned Teacher")]
    public int TeacherId { get; set; }

    [Required]
    [Display(Name = "Day of Week")]
    public string DayOfWeek { get; set; } = "Sunday";

    [Required]
    [Range(1, 8, ErrorMessage = "Period must be between 1 and 8")]
    [Display(Name = "Period Number")]
    public int PeriodNumber { get; set; } = 1;

    [Required]
    [Display(Name = "Start Time")]
    public string StartTime { get; set; } = "09:00 AM";

    [Required]
    [Display(Name = "End Time")]
    public string EndTime { get; set; } = "09:45 AM";

    [Display(Name = "Room Number")]
    public string? RoomNumber { get; set; }

    public SelectList? Classes { get; set; }
    public SelectList? Sections { get; set; }
    public SelectList? Subjects { get; set; }
    public SelectList? Teachers { get; set; }
}

