using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SettingsController(
        ApplicationDbContext context,
        IConfiguration configuration,
        IAccountService accountService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _accountService = accountService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Sessions = await _context.AcademicSessions.OrderByDescending(s => s.Year).ToListAsync();
        ViewBag.SchoolSettings = _configuration.GetSection("SchoolSettings");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSession(string sessionName, int year, DateTime startDate, DateTime endDate, bool isCurrent)
    {
        if (string.IsNullOrWhiteSpace(sessionName))
        {
            TempData["ErrorMessage"] = "Session name is required.";
            return RedirectToAction(nameof(Index));
        }

        if (isCurrent)
        {
            var sessions = await _context.AcademicSessions.ToListAsync();
            foreach (var s in sessions) s.IsCurrent = false;
        }

        var session = new AcademicSession
        {
            SessionName = sessionName.Trim(),
            Year = year,
            StartDate = startDate,
            EndDate = endDate,
            IsCurrent = isCurrent
        };

        await _context.AcademicSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        var currentUserId = _userManager.GetUserId(User);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "AcademicSession", session.Id.ToString(), $"Added academic session {session.SessionName}");

        TempData["SuccessMessage"] = "Academic session configured successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCurrentSession(int id)
    {
        var sessions = await _context.AcademicSessions.ToListAsync();
        foreach (var s in sessions)
        {
            s.IsCurrent = (s.Id == id);
        }
        await _context.SaveChangesAsync();

        var currentUserId = _userManager.GetUserId(User);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Update", "AcademicSession", id.ToString(), $"Set active current academic session #{id}");

        TempData["SuccessMessage"] = "Current active academic session updated.";
        return RedirectToAction(nameof(Index));
    }
}

