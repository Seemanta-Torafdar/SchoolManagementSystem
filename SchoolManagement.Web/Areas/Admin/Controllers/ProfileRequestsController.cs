using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class ProfileRequestsController : Controller
{
    private readonly IProfileRequestService _requestService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileRequestsController(
        IProfileRequestService requestService,
        UserManager<ApplicationUser> userManager)
    {
        _requestService = requestService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _requestService.GetAllRequestsAsync();
        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, bool approve, string? adminRemarks)
    {
        var currentUserName = User.Identity?.Name ?? "Administrator";
        var result = await _requestService.ReviewRequestAsync(id, approve, adminRemarks, currentUserName);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

