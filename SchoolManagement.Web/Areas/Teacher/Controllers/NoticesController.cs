using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class NoticesController : Controller
{
    private readonly INoticeService _noticeService;

    public NoticesController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    public async Task<IActionResult> Index()
    {
        var notices = await _noticeService.GetTeacherNoticesAsync(20);
        return View(notices);
    }

    public async Task<IActionResult> Details(int id)
    {
        var notice = await _noticeService.GetByIdAsync(id, incrementView: true);
        if (notice == null) return NotFound();
        return View(notice);
    }
}

