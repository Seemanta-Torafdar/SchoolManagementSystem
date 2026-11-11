using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = UserRole.Teacher)]
public class RoutineController : Controller
{
    private readonly IRoutineService _routineService;
    private readonly ITeacherService _teacherService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoutineController(
        IRoutineService routineService,
        ITeacherService teacherService,
        UserManager<ApplicationUser> userManager)
    {
        _routineService = routineService;
        _teacherService = teacherService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var teacher = await _teacherService.GetByUserIdAsync(userId);
        if (teacher == null) return NotFound();

        var routines = await _routineService.GetRoutineByTeacherAsync(teacher.Id);
        return View(routines);
    }
}

