using SchoolManagement.BLL.DTOs.Notice;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Web.ViewModels.Notice;

public class NoticeListViewModel
{
    public PaginatedList<NoticeDto> Notices { get; set; } = null!;
    public string? SearchTerm { get; set; }
    public NoticeType? Category { get; set; }
    public NoticeStatus? Status { get; set; }
}

public class NoticeCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Notice Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [Display(Name = "Notice Details")]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public NoticeType Category { get; set; } = NoticeType.General;

    [Required]
    [Display(Name = "Publication Status")]
    public NoticeStatus Status { get; set; } = NoticeStatus.Published;

    [DataType(DataType.Date)]
    [Display(Name = "Publish Date")]
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;

    [DataType(DataType.Date)]
    [Display(Name = "Expiry Date (Optional)")]
    public DateTime? ExpiryDate { get; set; }

    [Display(Name = "Visible to Public Website")]
    public bool IsPublic { get; set; } = true;

    [Display(Name = "Teacher Only Notice (Internal)")]
    public bool IsTeacherOnly { get; set; } = false;

    public string? ExistingAttachmentUrl { get; set; }
    public IFormFile? AttachmentFile { get; set; }
}

