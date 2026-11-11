using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Examination;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class ExaminationsController : Controller
{
    private readonly IExaminationService _examService;
    private readonly ISubjectService _subjectService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExaminationsController(
        IExaminationService examService,
        ISubjectService subjectService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _examService = examService;
        _subjectService = subjectService;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var exams = await _examService.GetAllAsync();
        ViewBag.Sessions = await _context.AcademicSessions.OrderByDescending(s => s.Year).ToListAsync();
        return View(exams);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExaminationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            TempData["ErrorMessage"] = "Examination name is required.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _examService.CreateAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Examination created successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ManageSubjects(int id)
    {
        var exam = await _examService.GetByIdAsync(id);
        if (exam == null) return NotFound();

        var examSubjects = await _examService.GetExamSubjectsAsync(id);
        var allSubjects = await _subjectService.GetAllAsync();

        ViewBag.Exam = exam;
        ViewBag.AllSubjects = allSubjects;
        return View(examSubjects);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExamSubject(CreateExamSubjectDto dto)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _examService.AddExamSubjectAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Subject added to examination schedule.";
        }
        return RedirectToAction(nameof(ManageSubjects), new { id = dto.ExaminationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExamSubject(int examSubjectId, int examId)
    {
        var currentUserId = _userManager.GetUserId(User);
        await _examService.DeleteExamSubjectAsync(examSubjectId, currentUserId);
        TempData["SuccessMessage"] = "Subject removed from examination schedule.";
        return RedirectToAction(nameof(ManageSubjects), new { id = examId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id, bool publish)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _examService.PublishExamResultsAsync(id, publish, currentUserId);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _examService.DeleteAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Examination deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}

