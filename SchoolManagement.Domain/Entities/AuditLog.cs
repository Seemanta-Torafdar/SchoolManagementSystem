using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Publish, etc.
    public string EntityName { get; set; } = string.Empty; // Student, Examination, Result, etc.
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

