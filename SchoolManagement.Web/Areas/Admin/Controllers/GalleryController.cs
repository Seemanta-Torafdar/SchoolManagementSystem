using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class GalleryController : Controller
{
    private readonly IGalleryService _galleryService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public GalleryController(
        IGalleryService galleryService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _galleryService = galleryService;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _galleryService.GetAllGalleryAsync();
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(GalleryDto dto, IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select an image file to upload.";
            return RedirectToAction(nameof(Index));
        }

        if (!FileValidationHelper.IsValidImage(imageFile.FileName, imageFile.Length, out var error))
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Index));
        }

        var folder = Path.Combine(_env.WebRootPath, "uploads", "gallery");
        Directory.CreateDirectory(folder);
        var fileName = FileValidationHelper.GenerateUniqueFileName(imageFile.FileName);
        using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        dto.ImageUrl = $"/uploads/gallery/{fileName}";
        dto.IsActive = true;

        var currentUserId = _userManager.GetUserId(User);
        var result = await _galleryService.CreateAsync(dto, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Photo uploaded to gallery successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        await _galleryService.DeleteAsync(id, currentUserId);
        TempData["SuccessMessage"] = "Photo removed from gallery.";
        return RedirectToAction(nameof(Index));
    }
}

