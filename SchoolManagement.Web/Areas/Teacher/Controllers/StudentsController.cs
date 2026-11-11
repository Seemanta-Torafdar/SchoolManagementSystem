using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class StudentsController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IStudentService _studentService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsController(
        ITeacherService teacherService,
        ISubjectService subjectService,
        IStudentService studentService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _teacherService = teacherService;
        _subjectService = subjectService;
        _studentService = studentService;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? classId, int? sectionId)
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        var subjects = await _subjectService.GetByTeacherIdAsync(teacher.Id);
        var assignedClassIds = subjects.Select(s => s.SchoolClassId).Distinct().ToList();

        var assignedClasses = await _context.SchoolClasses
            .Include(c => c.Sections)
            .Where(c => assignedClassIds.Contains(c.Id))
            .ToListAsync();

        ViewBag.AssignedClasses = assignedClasses;

        var selectedClassId = classId ?? assignedClassIds.FirstOrDefault();
        if (selectedClassId > 0)
        {
            var currentSession = await _context.AcademicSessions.FirstOrDefaultAsync(s => s.IsCurrent) 
                                 ?? await _context.AcademicSessions.FirstAsync();

            var sections = assignedClasses.FirstOrDefault(c => c.Id == selectedClassId)?.Sections.ToList() 
                           ?? new List<Section>();
            ViewBag.Sections = sections;

            var selectedSectionId = sectionId ?? sections.FirstOrDefault()?.Id ?? 0;
            ViewBag.SelectedClassId = selectedClassId;
            ViewBag.SelectedSectionId = selectedSectionId;

            if (selectedSectionId > 0)
            {
                var students = await _studentService.GetStudentsByClassAndSectionAsync(selectedClassId, selectedSectionId, currentSession.Id);
                return View(students);
            }
        }

        return View(new List<SchoolManagement.BLL.DTOs.Student.StudentDto>());
    }
}

