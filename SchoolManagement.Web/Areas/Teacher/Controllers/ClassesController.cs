using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class ClassesController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClassesController(
        ITeacherService teacherService,
        ISubjectService subjectService,
        UserManager<ApplicationUser> userManager)
    {
        _teacherService = teacherService;
        _subjectService = subjectService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        var subjects = await _subjectService.GetByTeacherIdAsync(teacher.Id);
        return View(subjects);
    }
}

