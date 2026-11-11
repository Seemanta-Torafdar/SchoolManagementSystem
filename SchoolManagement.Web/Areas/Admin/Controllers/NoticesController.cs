using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.DTOs.Notice;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Web.ViewModels.Notice;

namespace SchoolManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = UserRole.Admin)]
public class NoticesController : Controller
{
    private readonly INoticeService _noticeService;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public NoticesController(
        INoticeService noticeService,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _noticeService = noticeService;
        _env = env;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, NoticeType? category, NoticeStatus? status, int page = 1)
    {
        var pageSize = 10;
        var paginated = await _noticeService.SearchNoticesAsync(searchTerm, category, status, page, pageSize);

        var model = new NoticeListViewModel
        {
            Notices = paginated,
            SearchTerm = searchTerm,
            Category = category,
            Status = status
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NoticeCreateEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoticeCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? attachmentUrl = null;
        if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidDocument(model.AttachmentFile.FileName, model.AttachmentFile.Length, out var error))
            {
                ModelState.AddModelError("AttachmentFile", error);
                return View(model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "notices");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = FileValidationHelper.GenerateUniqueFileName(model.AttachmentFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.AttachmentFile.CopyToAsync(stream);
            }
            attachmentUrl = $"/uploads/notices/{uniqueFileName}";
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _noticeService.CreateAsync(new CreateNoticeDto
        {
            Title = model.Title,
            Content = model.Content,
            Category = model.Category,
            Status = model.Status,
            PublishDate = model.PublishDate,
            ExpiryDate = model.ExpiryDate,
            IsPublic = model.IsPublic,
            IsTeacherOnly = model.IsTeacherOnly,
            AttachmentUrl = attachmentUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Notice created and published successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var notice = await _noticeService.GetByIdAsync(id);
        if (notice == null) return NotFound();

        var model = new NoticeCreateEditViewModel
        {
            Id = notice.Id,
            Title = notice.Title,
            Content = notice.Content,
            Category = notice.Category,
            Status = notice.Status,
            PublishDate = notice.PublishDate,
            ExpiryDate = notice.ExpiryDate,
            IsPublic = notice.IsPublic,
            IsTeacherOnly = notice.IsTeacherOnly,
            ExistingAttachmentUrl = notice.AttachmentUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NoticeCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? attachmentUrl = model.ExistingAttachmentUrl;
        if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
        {
            if (!FileValidationHelper.IsValidDocument(model.AttachmentFile.FileName, model.AttachmentFile.Length, out var error))
            {
                ModelState.AddModelError("AttachmentFile", error);
                return View(model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "notices");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = FileValidationHelper.GenerateUniqueFileName(model.AttachmentFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.AttachmentFile.CopyToAsync(stream);
            }
            attachmentUrl = $"/uploads/notices/{uniqueFileName}";
        }

        var currentUserId = _userManager.GetUserId(User);
        var result = await _noticeService.UpdateAsync(new UpdateNoticeDto
        {
            Id = model.Id,
            Title = model.Title,
            Content = model.Content,
            Category = model.Category,
            Status = model.Status,
            PublishDate = model.PublishDate,
            ExpiryDate = model.ExpiryDate,
            IsPublic = model.IsPublic,
            IsTeacherOnly = model.IsTeacherOnly,
            AttachmentUrl = attachmentUrl
        }, currentUserId);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Notice updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = _userManager.GetUserId(User);
        var result = await _noticeService.DeleteAsync(id, currentUserId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        else
        {
            TempData["SuccessMessage"] = "Notice deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}

