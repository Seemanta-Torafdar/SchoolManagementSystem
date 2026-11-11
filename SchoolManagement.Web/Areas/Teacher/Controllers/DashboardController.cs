using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Dashboard;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class DashboardController : Controller
{
    private readonly IAccountService _accountService;
    private readonly INoticeService _noticeService;
    private readonly ITeacherService _teacherService;
    private readonly IRoutineService _routineService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IAccountService accountService,
        INoticeService noticeService,
        ITeacherService teacherService,
        IRoutineService routineService,
        UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _noticeService = noticeService;
        _teacherService = teacherService;
        _routineService = routineService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var stats = await _accountService.GetTeacherDashboardStatsAsync(userId);
        var notices = await _noticeService.GetTeacherNoticesAsync(5);

        var teacher = await _teacherService.GetByUserIdAsync(userId);
        var routines = teacher != null 
            ? await _routineService.GetRoutineByTeacherAsync(teacher.Id) 
            : new List<SchoolManagement.BLL.DTOs.Routine.RoutineDto>();

        var model = new TeacherDashboardViewModel
        {
            Stats = stats,
            Notices = notices,
            TodayRoutines = routines
        };

        return View(model);
    }
}

