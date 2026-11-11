using SchoolManagement.BLL.DTOs.Account;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IAccountService
{
    Task<ResultResponse<UserDto>> ValidateUserCredentialsAsync(string email, string password);
    Task<ResultResponse> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<IReadOnlyList<UserDto>> GetAllUsersAsync();
    Task<ResultResponse> ToggleUserStatusAsync(string userId, string? currentUserId = null);
    Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync();
    Task<TeacherDashboardStatsDto> GetTeacherDashboardStatsAsync(string userId);
    Task<IReadOnlyList<AuditLogDto>> GetRecentAuditLogsAsync(int count = 20);
    Task LogAuditAsync(string? userId, string userName, string action, string entityName, string? entityId = null, string? details = null, string? ipAddress = null);
}
