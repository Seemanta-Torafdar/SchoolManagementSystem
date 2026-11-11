using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoticesController : ControllerBase
{
    private readonly INoticeService _noticeService;

    public NoticesController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicNotices([FromQuery] int count = 10)
    {
        var notices = await _noticeService.GetPublicNoticesAsync(count);
        return Ok(notices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var notice = await _noticeService.GetByIdAsync(id, incrementView: true);
        if (notice == null || !notice.IsPublic)
        {
            return NotFound(new { message = "Notice not found." });
        }

        return Ok(notice);
    }
}

