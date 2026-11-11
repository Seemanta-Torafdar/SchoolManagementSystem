using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class ManagementController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountService _accountService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagementController(
        ApplicationDbContext context,
        IAccountService accountService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _accountService = accountService;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _context.ManagementMembers
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
        return View(members);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ManagementMember member, IFormFile? photoFile)
    {
        if (photoFile != null && photoFile.Length > 0)
        {
            if (FileValidationHelper.IsValidImage(photoFile.FileName, photoFile.Length, out _))
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads", "management");
                Directory.CreateDirectory(folder);
                var fileName = FileValidationHelper.GenerateUniqueFileName(photoFile.FileName);
                using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }
                member.PhotoUrl = $"/uploads/management/{fileName}";
            }
        }

        await _context.ManagementMembers.AddAsync(member);
        await _context.SaveChangesAsync();

        var currentUserId = _userManager.GetUserId(User);
        await _accountService.LogAuditAsync(currentUserId, "Admin", "Create", "ManagementMember", member.Id.ToString(), $"Added management committee member {member.Name}");

        TempData["SuccessMessage"] = "Management member added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _context.ManagementMembers.FindAsync(id);
        if (member != null)
        {
            member.IsDeleted = true;
            await _context.SaveChangesAsync();

            var currentUserId = _userManager.GetUserId(User);
            await _accountService.LogAuditAsync(currentUserId, "Admin", "Delete", "ManagementMember", id.ToString(), $"Deleted management member {member.Name}");
        }

        TempData["SuccessMessage"] = "Management member removed.";
        return RedirectToAction(nameof(Index));
    }
}

