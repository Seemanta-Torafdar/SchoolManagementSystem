using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Web.Controllers;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitMessage(string name, string email, string phone, string subject, string message)
    {
        TempData["SuccessMessage"] = "Thank you for contacting us! Your inquiry has been submitted to the school administration.";
        return RedirectToAction("Index");
    }
}

