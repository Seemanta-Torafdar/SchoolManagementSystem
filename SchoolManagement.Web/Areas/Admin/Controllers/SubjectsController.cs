using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Subject;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class SubjectsController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly IClassService _classService;
    private readonly ITeacherService _teacherService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubjectsController(
        ISubjectService subjectService,
        IClassService classService,
        ITeacherService teacherService,
        UserManager<ApplicationUser> userManager)
    {
        _subjectService = subjectService;
        _classService = classService;
        _teacherService = teacherService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var subjects = await _subjectService.GetAllAsync();
        ViewBag.Classes = await _classService.GetAllClassesAsync();
        ViewBag.Teachers = await _teacherService.GetPublicTeachersAsync();
        return View(subjects);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSubjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.SubjectCode))
        {
            TempData["ErrorMessage"] = "Subject Name and Code are required.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _subjectService.CreateAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Subject created successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateSubjectDto dto)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _subjectService.UpdateAsync(id, dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Subject updated successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _subjectService.DeleteAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Subject deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}

