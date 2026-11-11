using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class ProfileUpdateRequest : AuditableEntity
{
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public string RequestedFirstName { get; set; } = string.Empty;
    public string RequestedLastName { get; set; } = string.Empty;
    public string RequestedDesignation { get; set; } = string.Empty;
    public string RequestedQualification { get; set; } = string.Empty;
    public int RequestedExperienceYears { get; set; }
    public string? Reason { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string? AdminRemarks { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
}

