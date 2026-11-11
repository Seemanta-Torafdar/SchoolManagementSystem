using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Teacher;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Teacher;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class TeachersController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeachersController(
        ITeacherService teacherService,
        ISubjectService subjectService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _teacherService = teacherService;
        _subjectService = subjectService;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? subjectId, bool? isActive, int page = 1)
    {
        var pageSize = 10;
        var paginated = await _teacherService.SearchTeachersAsync(searchTerm, subjectId, isActive, page, pageSize);

        var subjects = await _subjectService.GetAllAsync();
        var model = new TeacherListViewModel
        {
            Teachers = paginated,
            SearchTerm = searchTerm,
            SubjectId = subjectId,
            IsActive = isActive,
            Subjects = new SelectList(subjects, "Id", "Name", subjectId)
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);
        if (teacher == null) return NotFound();
        return View(teacher);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new TeacherCreateEditViewModel
        {
            TeacherIdNumber = $"TCH-{new Random().Next(1000, 9999)}"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? photoUrl = null;
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidImage(model.PhotoFile.FileName, model.PhotoFile.Length, out var error))
            {
                ModelState.AddModelError("PhotoFile", error);
                return View(model);
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

        var currentUserId = _userManager.GetUserId(User);
        var result = await _teacherService.CreateAsync(new CreateTeacherDto
        {
            TeacherIdNumber = model.TeacherIdNumber ?? $"TCH-{new Random().Next(1000, 9999)}",
            FirstName = model.FirstName,
            LastName = model.LastName,
            Designation = model.Designation,
            Qualification = model.Qualification,
            ExperienceYears = model.ExperienceYears,
            Phone = model.Phone,
            Email = model.Email,
            Gender = model.Gender,
            JoiningDate = model.JoiningDate,
            Address = model.Address,
            Bio = model.Bio,
            IsActive = model.IsActive,
            IsPubliclyVisible = model.IsPubliclyVisible,
            TemporaryPassword = model.TemporaryPassword,
            ProfilePictureUrl = photoUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Teacher account and profile created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);
        if (teacher == null) return NotFound();

        var model = new TeacherCreateEditViewModel
        {
            Id = teacher.Id,
            TeacherIdNumber = teacher.TeacherIdNumber,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Designation = teacher.Designation,
            Qualification = teacher.Qualification,
            ExperienceYears = teacher.ExperienceYears,
            Phone = teacher.Phone,
            Email = teacher.Email,
            Gender = teacher.Gender,
            JoiningDate = teacher.JoiningDate,
            Address = teacher.Address,
            Bio = teacher.Bio,
            IsActive = teacher.IsActive,
            IsPubliclyVisible = teacher.IsPubliclyVisible,
            ExistingPhotoUrl = teacher.ProfilePictureUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TeacherCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? photoUrl = model.ExistingPhotoUrl;
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidImage(model.PhotoFile.FileName, model.PhotoFile.Length, out var error))
            {
                ModelState.AddModelError("PhotoFile", error);
                return View(model);
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

        var currentUserId = _userManager.GetUserId(User);
        var result = await _teacherService.UpdateAsync(new UpdateTeacherDto
        {
            Id = model.Id,
            TeacherIdNumber = model.TeacherIdNumber ?? "",
            FirstName = model.FirstName,
            LastName = model.LastName,
            Designation = model.Designation,
            Qualification = model.Qualification,
            ExperienceYears = model.ExperienceYears,
            Phone = model.Phone,
            Email = model.Email,
            Gender = model.Gender,
            JoiningDate = model.JoiningDate,
            Address = model.Address,
            Bio = model.Bio,
            IsActive = model.IsActive,
            IsPubliclyVisible = model.IsPubliclyVisible,
            ProfilePictureUrl = photoUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Teacher updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _teacherService.ToggleStatusAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}

