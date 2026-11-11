using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class ProfileRequestsController : Controller
{
    private readonly IProfileRequestService _requestService;
    private readonly ITeacherService _teacherService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileRequestsController(
        IProfileRequestService requestService,
        ITeacherService teacherService,
        UserManager<ApplicationUser> userManager)
    {
        _requestService = requestService;
        _teacherService = teacherService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        var requests = await _requestService.GetRequestsByTeacherIdAsync(teacher.Id);
        ViewBag.Teacher = teacher;
        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(CreateProfileRequestDto dto)
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        dto.TeacherId = teacher.Id;
        var result = await _requestService.SubmitRequestAsync(dto);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Profile update request has been submitted to the Administrator for verification.";
        }

        return RedirectToAction(nameof(Index));
    }
}

