using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Web.Controllers;

public class ShowcaseController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}

