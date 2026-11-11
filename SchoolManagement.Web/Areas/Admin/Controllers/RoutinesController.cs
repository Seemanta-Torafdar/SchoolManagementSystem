using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.BLL.DTOs.Routine;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Routine;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class RoutinesController : Controller
{
    private readonly IRoutineService _routineService;
    private readonly IClassService _classService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoutinesController(
        IRoutineService routineService,
        IClassService classService,
        ISubjectService subjectService,
        ITeacherService teacherService,
        UserManager<ApplicationUser> userManager)
    {
        _routineService = routineService;
        _classService = classService;
        _subjectService = subjectService;
        _teacherService = teacherService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? classId, int? sectionId)
    {
        var classes = await _classService.GetAllClassesAsync();
        var selectedClassId = classId ?? classes.FirstOrDefault()?.Id ?? 0;

        var model = new RoutineListViewModel
        {
            ClassId = selectedClassId,
            Classes = new SelectList(classes, "Id", "Name", selectedClassId)
        };

        if (selectedClassId > 0)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(selectedClassId);
            var selectedSectionId = sectionId ?? sections.FirstOrDefault()?.Id ?? 0;
            model.SectionId = selectedSectionId;
            model.Sections = new SelectList(sections, "Id", "Name", selectedSectionId);

            if (selectedSectionId > 0)
            {
                model.Routines = await _routineService.GetRoutineByClassAndSectionAsync(selectedClassId, selectedSectionId);
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? classId, int? sectionId)
    {
        var model = new RoutineCreateEditViewModel
        {
            SchoolClassId = classId ?? 0,
            SectionId = sectionId ?? 0
        };
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoutineCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _routineService.CreateAsync(new CreateRoutineDto
        {
            SchoolClassId = model.SchoolClassId,
            SectionId = model.SectionId,
            SubjectId = model.SubjectId,
            TeacherId = model.TeacherId,
            DayOfWeek = model.DayOfWeek,
            PeriodNumber = model.PeriodNumber,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            RoomNumber = model.RoomNumber
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = "Class routine entry created successfully.";
        return RedirectToAction(nameof(Index), new { classId = model.SchoolClassId, sectionId = model.SectionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int classId, int sectionId)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _routineService.DeleteAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Routine entry removed successfully.";
        }
        return RedirectToAction(nameof(Index), new { classId, sectionId });
    }

    private async Task PopulateDropdownsAsync(RoutineCreateEditViewModel model)
    {
        var classes = await _classService.GetAllClassesAsync();
        model.Classes = new SelectList(classes, "Id", "Name", model.SchoolClassId);

        var classId = model.SchoolClassId > 0 ? model.SchoolClassId : (classes.FirstOrDefault()?.Id ?? 0);
        if (classId > 0)
        {
            var sections = await _classService.GetSectionsByClassIdAsync(classId);
            model.Sections = new SelectList(sections, "Id", "Name", model.SectionId);

            var subjects = await _subjectService.GetByClassIdAsync(classId);
            model.Subjects = new SelectList(subjects, "Id", "Name", model.SubjectId);
        }

        var teachers = await _teacherService.GetPublicTeachersAsync();
        model.Teachers = new SelectList(teachers, "Id", "FullName", model.TeacherId);
    }
}

