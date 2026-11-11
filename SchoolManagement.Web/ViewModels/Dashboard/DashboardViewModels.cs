using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.DTOs.Notice;
using SchoolManagement.BLL.DTOs.Routine;

namespace SchoolManagement.Web.ViewModels.Dashboard;

public class AdminDashboardViewModel
{
    public AdminDashboardStatsDto Stats { get; set; } = new();
    public IReadOnlyList<NoticeDto> RecentNotices { get; set; } = new List<NoticeDto>();
}

public class TeacherDashboardViewModel
{
    public TeacherDashboardStatsDto Stats { get; set; } = new();
    public IReadOnlyList<NoticeDto> Notices { get; set; } = new List<NoticeDto>();
    public IReadOnlyList<RoutineDto> TodayRoutines { get; set; } = new List<RoutineDto>();
}

