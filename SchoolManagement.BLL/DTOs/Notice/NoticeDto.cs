using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.DTOs.Notice;

public class NoticeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoticeType Category { get; set; }
    public NoticeStatus Status { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? AttachmentUrl { get; set; }
    public bool IsPublic { get; set; }
    public bool IsTeacherOnly { get; set; }
    public int ViewCount { get; set; }
}

public class CreateNoticeDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoticeType Category { get; set; } = NoticeType.General;
    public NoticeStatus Status { get; set; } = NoticeStatus.Published;
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public string? AttachmentUrl { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsTeacherOnly { get; set; } = false;
}

public class UpdateNoticeDto : CreateNoticeDto
{
    public int Id { get; set; }
}

