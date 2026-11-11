using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class AuditLogsController : Controller
{
    private readonly IAccountService _accountService;

    public AuditLogsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _accountService.GetRecentAuditLogsAsync(50);
        return View(logs);
    }
}

