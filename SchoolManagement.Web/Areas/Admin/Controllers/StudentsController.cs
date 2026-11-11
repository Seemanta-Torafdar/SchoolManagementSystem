using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Student;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Student;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsController(
        IStudentService studentService,
        IClassService classService,
        ApplicationDbContext context,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _studentService = studentService;
        _classService = classService;
        _context = context;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? classId, int? sectionId, int? sessionId, int page = 1)
    {
        var pageSize = 10;
        var paginated = await _studentService.SearchStudentsAsync(searchTerm, classId, sectionId, sessionId, page, pageSize);

        var model = new StudentListViewModel
        {
            Students = paginated,
            SearchTerm = searchTerm,
            ClassId = classId,
            SectionId = sectionId,
            SessionId = sessionId
        };

        await PopulateFilterDropdownsAsync(model);
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new StudentCreateEditViewModel();
        await PopulateFormDropdownsAsync(model);
        model.AdmissionNumber = $"STD-{DateTime.UtcNow.Year}-{new Random().Next(100, 999)}";
        model.VerificationCode = new Random().Next(100000, 999999).ToString();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFormDropdownsAsync(model);
            return View(model);
        }

        string? photoUrl = null;
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidImage(model.PhotoFile.FileName, model.PhotoFile.Length, out var error))
            {
                ModelState.AddModelError("PhotoFile", error);
                await PopulateFormDropdownsAsync(model);
                return View(model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = FileValidationHelper.GenerateUniqueFileName(model.PhotoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(stream);
            }
            photoUrl = $"/uploads/students/{uniqueFileName}";
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _studentService.CreateAsync(new CreateStudentDto
        {
            AdmissionNumber = model.AdmissionNumber,
            RollNumber = model.RollNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
            FatherName = model.FatherName,
            MotherName = model.MotherName,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            BloodGroup = model.BloodGroup,
            Address = model.Address,
            GuardianPhone = model.GuardianPhone,
            GuardianEmail = model.GuardianEmail,
            SchoolClassId = model.SchoolClassId,
            SectionId = model.SectionId,
            AcademicSessionId = model.AcademicSessionId,
            Status = model.Status,
            VerificationCode = model.VerificationCode,
            PhotoUrl = photoUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateFormDropdownsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Student admitted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();

        var model = new StudentCreateEditViewModel
        {
            Id = student.Id,
            AdmissionNumber = student.AdmissionNumber,
            RollNumber = student.RollNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            FatherName = student.FatherName,
            MotherName = student.MotherName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            BloodGroup = student.BloodGroup,
            Address = student.Address,
            GuardianPhone = student.GuardianPhone,
            GuardianEmail = student.GuardianEmail,
            SchoolClassId = student.SchoolClassId,
            SectionId = student.SectionId,
            AcademicSessionId = student.AcademicSessionId,
            Status = student.Status,
            VerificationCode = student.VerificationCode,
            ExistingPhotoUrl = student.PhotoUrl
        };

        await PopulateFormDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFormDropdownsAsync(model);
            return View(model);
        }

        string? photoUrl = model.ExistingPhotoUrl;
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidImage(model.PhotoFile.FileName, model.PhotoFile.Length, out var error))
            {
                ModelState.AddModelError("PhotoFile", error);
                await PopulateFormDropdownsAsync(model);
                return View(model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = FileValidationHelper.GenerateUniqueFileName(model.PhotoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(stream);
            }
            photoUrl = $"/uploads/students/{uniqueFileName}";
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _studentService.UpdateAsync(new UpdateStudentDto
        {
            Id = model.Id,
            AdmissionNumber = model.AdmissionNumber,
            RollNumber = model.RollNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
            FatherName = model.FatherName,
            MotherName = model.MotherName,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            BloodGroup = model.BloodGroup,
            Address = model.Address,
            GuardianPhone = model.GuardianPhone,
            GuardianEmail = model.GuardianEmail,
            SchoolClassId = model.SchoolClassId,
            SectionId = model.SectionId,
            AcademicSessionId = model.AcademicSessionId,
            Status = model.Status,
            VerificationCode = model.VerificationCode,
            PhotoUrl = photoUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateFormDropdownsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Student updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _studentService.DeleteAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Student deactivated successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionsByClass(int classId)
    {
        var sections = await _classService.GetSectionsByClassIdAsync(classId);
        return Json(sections);
    }

    private async Task PopulateFilterDropdownsAsync(StudentListViewModel model)
    {
        var classes = await _classService.GetAllClassesAsync();
        model.Classes = new SelectList(classes, "Id", "Name", model.ClassId);

        if (model.ClassId.HasValue)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(model.ClassId.Value);
            model.Sections = new SelectList(sections, "Id", "Name", model.SectionId);
        }

        var sessions = await _context.AcademicSessions.OrderByDescending(s => s.Year).ToListAsync();
        model.Sessions = new SelectList(sessions, "Id", "SessionName", model.SessionId);
    }

    private async Task PopulateFormDropdownsAsync(StudentCreateEditViewModel model)
    {
        var classes = await _classService.GetAllClassesAsync();
        model.Classes = new SelectList(classes, "Id", "Name", model.SchoolClassId);

        var classId = model.SchoolClassId > 0 ? model.SchoolClassId : (classes.FirstOrDefault()?.Id ?? 0);
        if (classId > 0)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(classId);
            model.Sections = new SelectList(sections, "Id", "Name", model.SectionId);
        }

        var sessions = await _context.AcademicSessions.OrderByDescending(s => s.Year).ToListAsync();
        model.Sessions = new SelectList(sessions, "Id", "SessionName", model.AcademicSessionId);
    }
}

