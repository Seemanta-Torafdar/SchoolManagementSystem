using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.Web.Controllers;

public class AcademicsController : Controller
{
    private readonly IClassService _classService;
    private readonly ISubjectService _subjectService;

    public AcademicsController(IClassService classService, ISubjectService subjectService)
    {
        _classService = classService;
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
    {
        var classes = await _classService.GetAllClassesAsync();
        return View(classes);
    }

    public async Task<IActionResult> ClassDetails(int id)
    {
        var schoolClass = await _classService.GetByIdAsync(id);
        if (schoolClass == null) return NotFound();

        ViewBag.Subjects = await _subjectService.GetByClassIdAsync(id);
        return View(schoolClass);
    }
}

