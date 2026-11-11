using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Teacher;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Teacher;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class ProfileController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ProfileController(
        ITeacherService teacherService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _teacherService = teacherService;
        _userManager = userManager;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound("Teacher profile not found.");

        var model = new TeacherSelfProfileViewModel
        {
            Id = teacher.Id,
            FullName = teacher.FullName,
            Designation = teacher.Designation,
            Qualification = teacher.Qualification,
            ExperienceYears = teacher.ExperienceYears,
            Email = teacher.Email,
            Phone = teacher.Phone,
            Address = teacher.Address,
            Bio = teacher.Bio,
            ExistingPhotoUrl = teacher.ProfilePictureUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateContactInfo(TeacherSelfProfileViewModel model)
    {
        var userId = _userManager.GetUserId(User)!;

        string? photoUrl = model.ExistingPhotoUrl;
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidImage(model.PhotoFile.FileName, model.PhotoFile.Length, out var error))
            {
                ModelState.AddModelError("PhotoFile", error);
                return View("Index", model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "teachers");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = FileValidationHelper.GenerateUniqueFileName(model.PhotoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(stream);
            }
            photoUrl = $"/uploads/teachers/{uniqueFileName}";
        }

        var result = await _teacherService.UpdateSelfProfileAsync(new TeacherSelfProfileUpdateDto
        {
            Id = model.Id,
            Phone = model.Phone,
            Address = model.Address,
            Bio = model.Bio,
            ProfilePictureUrl = photoUrl
        }, userId);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Profile details updated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}

