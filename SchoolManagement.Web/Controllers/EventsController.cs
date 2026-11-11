using Microsoft.AspNetCore.Mvc;
using SchoolManagement.BLL.Interfaces;

namespace SchoolManagement.Web.Controllers;

public class EventsController : Controller
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllEventsAsync();
        return View(events);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        if (ev == null) return NotFound();
        return View(ev);
    }
}

