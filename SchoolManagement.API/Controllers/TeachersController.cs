using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet("directory")]
    public async Task<IActionResult> GetPublicFaculty()
    {
        var teachers = await _teacherService.GetPublicTeachersAsync();
        return Ok(teachers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);
        if (teacher == null || !teacher.IsPubliclyVisible)
        {
            return NotFound(new { message = "Teacher profile not found." });
        }

        return Ok(teacher);
    }
}

