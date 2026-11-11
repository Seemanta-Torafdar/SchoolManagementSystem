using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.Web.Controllers;

public class GalleryController : Controller
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    public async Task<IActionResult> Index(string? category = null)
    {
        var items = await _galleryService.GetActiveGalleryAsync(category);
        ViewBag.CurrentCategory = category;
        return View(items);
    }
}

