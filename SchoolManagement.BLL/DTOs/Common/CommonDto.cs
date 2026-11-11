using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.DTOs.Common;

public class AdminDashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }
    public int TotalActiveNotices { get; set; }
    public int TotalPendingRequests { get; set; }
    public Dictionary<string, int> StudentsPerClass { get; set; } = new();
    public List<AuditLogDto> RecentActivities { get; set; } = new();
}

public class TeacherDashboardStatsDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int AssignedClassesCount { get; set; }
    public int TotalAssignedStudentsCount { get; set; }
    public int ActiveNoticesCount { get; set; }
    public List<string> TodayRoutines { get; set; } = new();
}

public class ProfileRequestDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string CurrentDesignation { get; set; } = string.Empty;
    public string CurrentQualification { get; set; } = string.Empty;
    public int CurrentExperienceYears { get; set; }

    public string RequestedFirstName { get; set; } = string.Empty;
    public string RequestedLastName { get; set; } = string.Empty;
    public string RequestedDesignation { get; set; } = string.Empty;
    public string RequestedQualification { get; set; } = string.Empty;
    public int RequestedExperienceYears { get; set; }
    public string? Reason { get; set; }

    public RequestStatus Status { get; set; }
    public string? AdminRemarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
}

public class CreateProfileRequestDto
{
    public int TeacherId { get; set; }
    public string RequestedFirstName { get; set; } = string.Empty;
    public string RequestedLastName { get; set; } = string.Empty;
    public string RequestedDesignation { get; set; } = string.Empty;
    public string RequestedQualification { get; set; } = string.Empty;
    public int RequestedExperienceYears { get; set; }
    public string? Reason { get; set; }
}

public class GalleryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? EventDate { get; set; }
    public bool IsActive { get; set; }
}

public class ManagementMemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? PhotoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class AuditLogDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

