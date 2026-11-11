using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClasses()
    {
        var classes = await _classService.GetAllClassesAsync();
        return Ok(classes);
    }

    [HttpGet("{classId:int}/sections")]
    public async Task<IActionResult> GetSections(int classId)
    {
        var sections = await _classService.GetSectionsByClassIdAsync(classId);
        return Ok(sections);
    }
}

