using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;

namespace SchoolManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly INoticeService _noticeService;
    private readonly IEventService _eventService;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;
    private readonly ApplicationDbContext _context;

    public HomeController(
        INoticeService noticeService,
        IEventService eventService,
        ITeacherService teacherService,
        IStudentService studentService,
        IClassService classService,
        ApplicationDbContext context)
    {
        _noticeService = noticeService;
        _eventService = eventService;
        _teacherService = teacherService;
        _studentService = studentService;
        _classService = classService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.LatestNotices = await _noticeService.GetPublicNoticesAsync(5);
        ViewBag.UpcomingEvents = await _eventService.GetUpcomingEventsAsync(3);
        ViewBag.Teachers = (await _teacherService.GetPublicTeachersAsync()).Take(4).ToList();
        ViewBag.ManagementMembers = await _context.ManagementMembers
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .Take(6)
            .ToListAsync();
        ViewBag.Principal = await _context.ManagementMembers
            .FirstOrDefaultAsync(m => m.Designation.Contains("Principal") && m.IsActive);

        ViewBag.TotalStudents = await _studentService.GetTotalCountAsync();
        ViewBag.TotalTeachers = await _teacherService.GetTotalCountAsync();
        ViewBag.TotalClasses = await _classService.GetTotalCountAsync();

        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
