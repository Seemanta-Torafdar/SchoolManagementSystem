using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class Gallery : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = "Campus"; // Campus, Sports, Cultural, Achievements, Events
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? EventDate { get; set; }
    public bool IsActive { get; set; } = true;
}

