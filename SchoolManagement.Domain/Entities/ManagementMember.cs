using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class ManagementMember : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty; // Principal, Vice Principal, Managing Committee Member, etc.
    public string? Message { get; set; } // Message from Principal / Chairman
    public string? PhotoUrl { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

