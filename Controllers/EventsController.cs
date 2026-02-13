using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class EventsController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;

    public EventsController(SmartEventManagement_TicketingSystemContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> All(
        string searchCategory,
        string searchLocation,
        DateTime? searchDate,
        decimal? maxPrice)
    {
        var eventsQuery = _context.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchCategory))
        {
            eventsQuery = eventsQuery
                .Where(e => e.Category.Contains(searchCategory));
        }

        if (!string.IsNullOrWhiteSpace(searchLocation))
        {
            eventsQuery = eventsQuery
                .Where(e => e.Location.Contains(searchLocation));
        }

        if (searchDate.HasValue)
        {
            eventsQuery = eventsQuery
                .Where(e => e.EventDate.Date == searchDate.Value.Date);
        }

        if (maxPrice.HasValue)
        {
            eventsQuery = eventsQuery
                .Where(e => e.Price <= maxPrice.Value);
        }

        var model = new EventSearchViewModel
        {
            Events = await eventsQuery
                .OrderBy(e => e.EventDate)
                .ToListAsync(),
            SearchCategory = searchCategory,
            SearchLocation = searchLocation,
            SearchDate = searchDate,
            MaxPrice = maxPrice
        };

        return View(model);
    }

    // GET: Upcoming Events with Search
    public async Task<IActionResult> Upcoming(
        string searchCategory,
        string searchLocation,
        DateTime? searchDate,
        decimal? maxPrice)
    {
        var eventsQuery = _context.Events
            .Where(e => e.EventDate >= DateTime.Now)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchCategory))
        {
            eventsQuery = eventsQuery.Where(e => e.Category.Contains(searchCategory));
        }

        if (!string.IsNullOrEmpty(searchLocation))
        {
            eventsQuery = eventsQuery.Where(e => e.Location.Contains(searchLocation));
        }

        if (searchDate.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.EventDate.Date == searchDate.Value.Date);
        }

        if (maxPrice.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.Price <= maxPrice.Value);
        }

        var events = await eventsQuery
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        return View(events);
    }
}
