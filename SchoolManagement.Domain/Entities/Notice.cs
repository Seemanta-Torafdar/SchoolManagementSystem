using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Notice : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoticeType Category { get; set; } = NoticeType.General;
    public NoticeStatus Status { get; set; } = NoticeStatus.Published;
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public string? AttachmentUrl { get; set; } // PDF or image file path
    public bool IsPublic { get; set; } = true;
    public bool IsTeacherOnly { get; set; } = false;
    public int ViewCount { get; set; } = 0;
}

