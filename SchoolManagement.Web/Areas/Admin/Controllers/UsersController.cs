using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class UsersController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(IAccountService accountService, UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _accountService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string userId)
    {
        var currentUserId = _userManager.GetUserId(User);
        if (userId == currentUserId)
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own administrative account.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _accountService.ToggleUserStatusAsync(userId, currentUserId);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(", ", result.Errors.Select(e => e.Description));
        }
        else
        {
            var currentUserId = _userManager.GetUserId(User);
            await _accountService.LogAuditAsync(currentUserId, "Admin", "ResetPassword", "User", userId, $"Reset password for {user.Email}");
            TempData["SuccessMessage"] = $"Password for {user.Email} has been reset successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}

