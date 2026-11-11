using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Event : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = "School Auditorium";
    public string? BannerImageUrl { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsUpcoming => EventDate >= DateTime.UtcNow.Date;
}

