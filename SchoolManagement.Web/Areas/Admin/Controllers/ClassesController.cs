using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Class;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class ClassesController : Controller
{
    private readonly IClassService _classService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClassesController(IClassService classService, UserManager<ApplicationUser> userManager)
    {
        _classService = classService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var classes = await _classService.GetAllClassesAsync();
        return View(classes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClass(CreateClassDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            TempData["ErrorMessage"] = "Class name cannot be empty.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _classService.CreateClassAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Class added successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSection(CreateSectionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            TempData["ErrorMessage"] = "Section name cannot be empty.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _classService.CreateSectionAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Section added successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _classService.DeleteClassAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Class deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _classService.DeleteSectionAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Section deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}

