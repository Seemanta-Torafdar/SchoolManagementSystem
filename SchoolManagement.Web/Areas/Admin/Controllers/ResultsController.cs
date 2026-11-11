using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.DTOs.Result;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Result;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class ResultsController : Controller
{
    private readonly IResultService _resultService;
    private readonly IExaminationService _examService;
    private readonly IClassService _classService;
    private readonly ISubjectService _subjectService;
    private readonly IStudentService _studentService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResultsController(
        IResultService resultService,
        IExaminationService examService,
        IClassService classService,
        ISubjectService subjectService,
        IStudentService studentService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _resultService = resultService;
        _examService = examService;
        _classService = classService;
        _subjectService = subjectService;
        _studentService = studentService;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? examinationId, int? classId, int? sectionId)
    {
        var exams = await _examService.GetAllAsync();
        var classes = await _classService.GetAllClassesAsync();

        ViewBag.Examinations = new SelectList(exams, "Id", "Name", examinationId);
        ViewBag.Classes = new SelectList(classes, "Id", "Name", classId);

        if (classId.HasValue)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(classId.Value);
            ViewBag.Sections = new SelectList(sections, "Id", "Name", sectionId);
        }

        if (examinationId.HasValue && classId.HasValue)
        {
            var results = await _resultService.GetClassResultsAsync(examinationId.Value, classId.Value, sectionId);
            ViewBag.Results = results;
            ViewBag.SelectedExam = exams.FirstOrDefault(e => e.Id == examinationId.Value);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MarksEntry(int? examinationId, int? classId, int? sectionId, int? subjectId)
    {
        var model = new MarksEntryFilterViewModel
        {
            ExaminationId = examinationId ?? 0,
            ClassId = classId ?? 0,
            SectionId = sectionId ?? 0,
            SubjectId = subjectId ?? 0
        };

        await PopulateMarksDropdownsAsync(model);

        if (model.ExaminationId > 0 && model.ClassId > 0 && model.SectionId > 0 && model.SubjectId > 0)
        {
            // Fetch students of this class and section in current session
            var currentSession = await _context.AcademicSessions.FirstOrDefaultAsync(s => s.IsCurrent) 
                                 ?? await _context.AcademicSessions.FirstAsync();

            var students = await _studentService.GetStudentsByClassAndSectionAsync(model.ClassId, model.SectionId, currentSession.Id);
            var existingResults = await _context.StudentResults
                .Where(r => r.ExaminationId == model.ExaminationId && r.SubjectId == model.SubjectId)
                .ToDictionaryAsync(r => r.StudentId);

            var bulk = new BulkMarksEntryDto
            {
                ExaminationId = model.ExaminationId,
                ClassId = model.ClassId,
                SectionId = model.SectionId,
                SubjectId = model.SubjectId,
                MaxMarks = 100,
                MarksEntries = students.Select(s => new BulkMarksEntryItemDto
                {
                    StudentId = s.Id,
                    StudentName = s.FullName,
                    RollNumber = s.RollNumber,
                    MarksObtained = existingResults.TryGetValue(s.Id, out var res) ? res.MarksObtained : 0,
                    Remarks = existingResults.TryGetValue(s.Id, out var r2) ? r2.Remarks : null
                }).ToList()
            };

            model.BulkEntry = bulk;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveMarks(BulkMarksEntryDto bulkEntry)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _resultService.SaveBulkMarksAsync(bulkEntry, currentUserId);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(MarksEntry), new
        {
            examinationId = bulkEntry.ExaminationId,
            classId = bulkEntry.ClassId,
            sectionId = bulkEntry.SectionId,
            subjectId = bulkEntry.SubjectId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishResults(int examinationId, bool publish)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _resultService.PublishResultsAsync(examinationId, publish, currentUserId);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index), new { examinationId });
    }

    [HttpGet]
    public async Task<IActionResult> Marksheet(int studentId, int examinationId)
    {
        var result = await _resultService.GetStudentMarksheetByIdAsync(studentId, examinationId);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound("Student marksheet not found.");
        }

        return View(result.Data);
    }

    private async Task PopulateMarksDropdownsAsync(MarksEntryFilterViewModel model)
    {
        var exams = await _examService.GetAllAsync();
        model.Examinations = new SelectList(exams, "Id", "Name", model.ExaminationId);

        var classes = await _classService.GetAllClassesAsync();
        model.Classes = new SelectList(classes, "Id", "Name", model.ClassId);

        if (model.ClassId > 0)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(model.ClassId);
            model.Sections = new SelectList(sections, "Id", "Name", model.SectionId);

            var subjects = await _subjectService.GetByClassIdAsync(model.ClassId);
            model.Subjects = new SelectList(subjects, "Id", "Name", model.SubjectId);
        }
    }
}

