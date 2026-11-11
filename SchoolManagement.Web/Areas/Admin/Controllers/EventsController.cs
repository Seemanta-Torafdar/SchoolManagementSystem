using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Event;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(
        IEventService eventService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _eventService = eventService;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllEventsAsync();
        return View(events);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventDto dto, IFormFile? bannerFile)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            TempData["ErrorMessage"] = "Event title is required.";
            return RedirectToAction(nameof(Index));
        }

        if (bannerFile != null && bannerFile.Length > 0)
        {
            if (FileValidationHelper.IsValidImage(bannerFile.FileName, bannerFile.Length, out _))
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads", "events");
                Directory.CreateDirectory(folder);
                var fileName = FileValidationHelper.GenerateUniqueFileName(bannerFile.FileName);
                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await bannerFile.CopyToAsync(stream);
                }
                dto.BannerImageUrl = $"/uploads/events/{fileName}";
            }
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _eventService.CreateAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Event published successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        await _eventService.DeleteAsync(id, currentUserId);
        TempData["SuccessMessage"] = "Event deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

