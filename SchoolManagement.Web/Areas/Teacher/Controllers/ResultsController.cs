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

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class ResultsController : Controller
{
    private readonly IResultService _resultService;
    private readonly IExaminationService _examService;
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResultsController(
        IResultService resultService,
        IExaminationService examService,
        ITeacherService teacherService,
        ISubjectService subjectService,
        IStudentService studentService,
        IClassService classService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _resultService = resultService;
        _examService = examService;
        _teacherService = teacherService;
        _subjectService = subjectService;
        _studentService = studentService;
        _classService = classService;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? examinationId, int? classId, int? sectionId, int? subjectId)
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        var assignedSubjects = await _subjectService.GetByTeacherIdAsync(teacher.Id);
        var exams = await _examService.GetAllAsync();

        ViewBag.Examinations = new SelectList(exams, "Id", "Name", examinationId);
        ViewBag.Subjects = new SelectList(assignedSubjects, "Id", "Name", subjectId);

        var model = new MarksEntryFilterViewModel
        {
            ExaminationId = examinationId ?? 0,
            SubjectId = subjectId ?? 0,
            ClassId = classId ?? 0,
            SectionId = sectionId ?? 0
        };

        if (model.SubjectId > 0 && model.ExaminationId > 0)
        {
            var selectedSubject = assignedSubjects.FirstOrDefault(s => s.Id == model.SubjectId);
            if (selectedSubject != null)
            {
                model.ClassId = selectedSubject.SchoolClassId;
                var sections = await _classService.GetSectionsByClassIdAsync(selectedSubject.SchoolClassId);
                ViewBag.Sections = new SelectList(sections, "Id", "Name", sectionId);

                var selectedSectionId = sectionId ?? sections.FirstOrDefault()?.Id ?? 0;
                model.SectionId = selectedSectionId;

                if (selectedSectionId > 0)
                {
                    var currentSession = await _context.AcademicSessions.FirstOrDefaultAsync(s => s.IsCurrent) 
                                         ?? await _context.AcademicSessions.FirstAsync();

                    var students = await _studentService.GetStudentsByClassAndSectionAsync(model.ClassId, selectedSectionId, currentSession.Id);
                    var existingResults = await _context.StudentResults
                        .Where(r => r.ExaminationId == model.ExaminationId && r.SubjectId == model.SubjectId)
                        .ToDictionaryAsync(r => r.StudentId);

                    model.BulkEntry = new BulkMarksEntryDto
                    {
                        ExaminationId = model.ExaminationId,
                        ClassId = model.ClassId,
                        SectionId = selectedSectionId,
                        SubjectId = model.SubjectId,
                        MaxMarks = selectedSubject.TotalMarks,
                        MarksEntries = students.Select(s => new BulkMarksEntryItemDto
                        {
                            StudentId = s.Id,
                            StudentName = s.FullName,
                            RollNumber = s.RollNumber,
                            MarksObtained = existingResults.TryGetValue(s.Id, out var res) ? res.MarksObtained : 0,
                            Remarks = existingResults.TryGetValue(s.Id, out var r2) ? r2.Remarks : null
                        }).ToList()
                    };
                }
            }
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
            TempData["SuccessMessage"] = "Student examination marks recorded successfully.";
        }

        return RedirectToAction(nameof(Index), new
        {
            examinationId = bulkEntry.ExaminationId,
            classId = bulkEntry.ClassId,
            sectionId = bulkEntry.SectionId,
            subjectId = bulkEntry.SubjectId
        });
    }
}

