using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Notice;

namespace SchoolManagement.Web.Controllers;

public class NoticesController : Controller
{
    private readonly INoticeService _noticeService;

    public NoticesController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    public async Task<IActionResult> Index(string? searchTerm, NoticeType? category, int page = 1)
    {
        var pageSize = 8;
        var notices = await _noticeService.SearchNoticesAsync(searchTerm, category, NoticeStatus.Published, page, pageSize);

        var viewModel = new NoticeListViewModel
        {
            Notices = notices,
            SearchTerm = searchTerm,
            Category = category
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var notice = await _noticeService.GetByIdAsync(id, incrementView: true);
        if (notice == null || (!notice.IsPublic && !User.Identity!.IsAuthenticated))
            return NotFound();

        return View(notice);
    }
}

