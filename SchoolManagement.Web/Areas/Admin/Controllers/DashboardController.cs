using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Dashboard;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class DashboardController : Controller
{
    private readonly IAccountService _accountService;
    private readonly INoticeService _noticeService;

    public DashboardController(IAccountService accountService, INoticeService noticeService)
    {
        _accountService = accountService;
        _noticeService = noticeService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _accountService.GetAdminDashboardStatsAsync();
        var notices = await _noticeService.GetPublicNoticesAsync(5);

        var model = new AdminDashboardViewModel
        {
            Stats = stats,
            RecentNotices = notices
        };

        return View(model);
    }
}

