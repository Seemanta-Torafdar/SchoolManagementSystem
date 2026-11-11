using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Result;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly IResultService _resultService;

    public ResultsController(IResultService resultService)
    {
        _resultService = resultService;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyMarksheet([FromBody] ResultSearchQueryDto query)
    {
        if (query == null || query.ExaminationId <= 0 || query.ClassId <= 0 || query.RollNumber <= 0 || string.IsNullOrWhiteSpace(query.VerificationCode))
        {
            return BadRequest(new { message = "All verification parameters are required." });
        }

        var result = await _resultService.GetVerifiedStudentMarksheetAsync(query);
        if (!result.Succeeded)
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(result.Data);
    }

    [HttpGet("student/{studentId:int}/exam/{examinationId:int}")]
    public async Task<IActionResult> GetStudentMarksheet(int studentId, int examinationId)
    {
        var result = await _resultService.GetStudentMarksheetByIdAsync(studentId, examinationId);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound(new { message = "Marksheet record not found." });
        }

        return Ok(result.Data);
    }
}

