using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.Web.Controllers;

public class TeachersController : Controller
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    public async Task<IActionResult> Index()
    {
        var teachers = await _teacherService.GetPublicTeachersAsync();
        return View(teachers);
    }
}

