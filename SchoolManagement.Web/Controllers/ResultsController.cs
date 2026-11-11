using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Result;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Web.ViewModels.Result;

namespace SchoolManagement.Web.Controllers;

public class ResultsController : Controller
{
    private readonly IResultService _resultService;
    private readonly IExaminationService _examService;
    private readonly IClassService _classService;
    private readonly ApplicationDbContext _context;

    public ResultsController(
        IResultService resultService,
        IExaminationService examService,
        IClassService classService,
        ApplicationDbContext context)
    {
        _resultService = resultService;
        _examService = examService;
        _classService = classService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new PublicResultSearchViewModel();
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("ResultSearchLimiter")]
    public async Task<IActionResult> Index(PublicResultSearchViewModel model)
    {
        await PopulateDropdownsAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultResponse = await _resultService.GetVerifiedStudentMarksheetAsync(new ResultSearchQueryDto
        {
            ExaminationId = model.ExaminationId,
            AcademicSessionId = model.AcademicSessionId,
            ClassId = model.ClassId,
            RollNumber = model.RollNumber,
            VerificationCode = model.VerificationCode
        });

        if (!resultResponse.Succeeded)
        {
            model.ErrorMessage = resultResponse.Message;
            return View(model);
        }

        model.Marksheet = resultResponse.Data;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MarksheetPrint(int studentId, int examinationId)
    {
        var resultResponse = await _resultService.GetStudentMarksheetByIdAsync(studentId, examinationId);
        if (!resultResponse.Succeeded || resultResponse.Data == null)
        {
            return NotFound("Marksheet not found.");
        }

        return View(resultResponse.Data);
    }

    private async Task PopulateDropdownsAsync(PublicResultSearchViewModel model)
    {
        var exams = await _examService.GetAllAsync();
        model.Examinations = new SelectList(exams.Where(e => e.IsPublished), "Id", "Name", model.ExaminationId);

        var sessions = await _context.AcademicSessions.OrderByDescending(s => s.Year).ToListAsync();
        model.Sessions = new SelectList(sessions, "Id", "SessionName", model.AcademicSessionId);

        var classes = await _classService.GetAllClassesAsync();
        model.Classes = new SelectList(classes, "Id", "Name", model.ClassId);
    }
}

