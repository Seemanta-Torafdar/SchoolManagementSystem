using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Account;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Account;

namespace SchoolManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccountService _accountService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IAccountService accountService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _accountService = accountService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole(UserRole.Admin))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (User.IsInRole(UserRole.Teacher))
                return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });

            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var validateResult = await _accountService.ValidateUserCredentialsAsync(model.Email, model.Password);
        if (!validateResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, validateResult.Message);
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User account not found.");
            return View(model);
        }

        // Perform Cookie sign-in
        await _signInManager.SignInAsync(user, model.RememberMe);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (validateResult.Data?.Role == UserRole.Admin)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        else if (validateResult.Data?.Role == UserRole.Teacher)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        var result = await _accountService.ChangePasswordAsync(userId, new ChangePasswordDto
        {
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword,
            ConfirmPassword = model.ConfirmPassword
        });

        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err);
            }
            return View(model);
        }

        TempData["SuccessMessage"] = "Password has been successfully changed.";
        if (User.IsInRole(UserRole.Admin))
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        else
            return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });
    }
}

