using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Account;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ResultResponse<UserDto>> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user == null)
            return ResultResponse<UserDto>.Failure("Invalid email or password.");

        if (!user.IsActive)
            return ResultResponse<UserDto>.Failure("Your account has been deactivated. Please contact the administrator.");

        if (await _userManager.IsLockedOutAsync(user))
            return ResultResponse<UserDto>.Failure("Account is temporarily locked out due to multiple failed login attempts. Try again later.");

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user);
            return ResultResponse<UserDto>.Failure("Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? user.Role;

        await LogAuditAsync(user.Id, user.FullName, "Login", "Account", user.Id, "User signed in successfully");

        return ResultResponse<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            FullName = user.FullName,
            Role = role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        }, "Validation successful.");
    }

    public async Task<ResultResponse> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ResultResponse.Failure("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ResultResponse.Failure(errors, "Failed to change password.");
        }

        await LogAuditAsync(userId, user.FullName, "ChangePassword", "Account", userId, "User changed password.");
        return ResultResponse.Success("Password changed successfully.");
    }

    public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        var dtos = new List<UserDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            dtos.Add(new UserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                FullName = u.FullName,
                Role = roles.FirstOrDefault() ?? u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            });
        }

        return dtos;
    }

    public async Task<ResultResponse> ToggleUserStatusAsync(string userId, string? currentUserId = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return ResultResponse.Failure("User not found.");

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        await LogAuditAsync(currentUserId, "Admin", "ToggleStatus", "User", userId, $"User account status set to {(user.IsActive ? "Active" : "Inactive")}");
        return ResultResponse.Success($"User account {(user.IsActive ? "activated" : "deactivated")}.");
    }

    public async Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync()
    {
        var totalStudents = await _context.Students.CountAsync();
        var totalTeachers = await _context.Teachers.CountAsync(t => t.IsActive);
        var totalClasses = await _context.SchoolClasses.CountAsync();
        var totalNotices = await _context.Notices.CountAsync(n => n.Status == NoticeStatus.Published);
        var pendingRequests = await _context.ProfileUpdateRequests.CountAsync(p => p.Status == RequestStatus.Pending);

        // Student distribution per class for Chart.js
        var classDistribution = await _context.SchoolClasses
            .Include(c => c.Students)
            .OrderBy(c => c.NumericValue)
            .ToDictionaryAsync(c => c.Name, c => c.Students.Count);

        var recentActivities = await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(7)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.UserName,
                Action = a.Action,
                EntityName = a.EntityName,
                Details = a.Details,
                Timestamp = a.Timestamp
            })
            .ToListAsync();

        return new AdminDashboardStatsDto
        {
            TotalStudents = totalStudents,
            TotalTeachers = totalTeachers,
            TotalClasses = totalClasses,
            TotalActiveNotices = totalNotices,
            TotalPendingRequests = pendingRequests,
            StudentsPerClass = classDistribution,
            RecentActivities = recentActivities
        };
    }

    public async Task<TeacherDashboardStatsDto> GetTeacherDashboardStatsAsync(string userId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
                .ThenInclude(s => s.SchoolClass)
            .Include(t => t.Routines)
                .ThenInclude(r => r.SchoolClass)
            .Include(t => t.Routines)
                .ThenInclude(r => r.Section)
            .Include(t => t.Routines)
                .ThenInclude(r => r.Subject)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return new TeacherDashboardStatsDto
            {
                TeacherName = "Teacher",
                Designation = "Faculty Member"
            };
        }

        var classIds = teacher.Subjects.Select(s => s.SchoolClassId).Distinct().ToList();
        var studentCount = await _context.Students.CountAsync(s => classIds.Contains(s.SchoolClassId));
        var noticeCount = await _context.Notices.CountAsync(n => (n.IsTeacherOnly || n.IsPublic) && n.Status == NoticeStatus.Published);

        var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();
        var todayRoutines = teacher.Routines
            .Where(r => r.DayOfWeek.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.PeriodNumber)
            .Select(r => $"Period {r.PeriodNumber}: {r.Subject?.Name} ({r.SchoolClass?.Name} - {r.Section?.Name}) at {r.StartTime} in {r.RoomNumber}")
            .ToList();

        return new TeacherDashboardStatsDto
        {
            TeacherId = teacher.Id,
            TeacherName = teacher.FullName,
            Designation = teacher.Designation,
            AssignedClassesCount = classIds.Count,
            TotalAssignedStudentsCount = studentCount,
            ActiveNoticesCount = noticeCount,
            TodayRoutines = todayRoutines
        };
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetRecentAuditLogsAsync(int count = 20)
    {
        var logs = await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();

        return logs.Select(a => new AuditLogDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.UserName,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Details = a.Details,
            IpAddress = a.IpAddress,
            Timestamp = a.Timestamp
        }).ToList();
    }

    public async Task LogAuditAsync(string? userId, string userName, string action, string entityName, string? entityId = null, string? details = null, string? ipAddress = null)
    {
        try
        {
            var log = new AuditLog
            {
                UserId = userId,
                UserName = string.IsNullOrWhiteSpace(userName) ? "System" : userName,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Fail-safe: audit logging should not bring down the main user operation
        }
    }
}
