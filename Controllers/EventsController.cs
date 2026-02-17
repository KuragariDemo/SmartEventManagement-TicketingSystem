using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models.Events;

[Authorize]
public class EventsController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;

    public EventsController(
     SmartEventManagement_TicketingSystemContext context,
     UserManager<SmartEventManagement_TicketingSystemUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ===============================
    // ALL EVENTS
    // ===============================
    public async Task<IActionResult> All(
        string searchCategory,
        string searchLocation,
        DateTime? searchDate,
        decimal? maxPrice)
    {
        var eventsQuery = BuildSearchQuery(searchCategory, searchLocation, searchDate, maxPrice);

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

    // ===============================
    // UPCOMING EVENTS
    // ===============================
    public async Task<IActionResult> Upcoming(
        string searchCategory,
        string searchLocation,
        DateTime? searchDate,
        decimal? maxPrice)
    {
        var eventsQuery = BuildSearchQuery(searchCategory, searchLocation, searchDate, maxPrice)
            .Where(e => e.EventDate >= DateTime.Now);

        var events = await eventsQuery
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        return View(events);
    }

    // ===============================
    // SEARCH LOGIC (Reusable)
    // ===============================
    private IQueryable<Event> BuildSearchQuery(
    string searchCategory,
    string searchLocation,
    DateTime? searchDate,
    decimal? maxPrice)
    {
        var query = _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchCategory))
        {
            string category = searchCategory.Trim();

            query = query.Where(e =>
    (e.Category != null && EF.Functions.Like(e.Category.Name, $"%{category}%")) ||
    EF.Functions.Like(e.Title, $"%{category}%"));

        }

        if (!string.IsNullOrWhiteSpace(searchLocation))
        {
            string location = searchLocation.Trim();

            query = query.Where(e =>
                e.Venue != null &&
                EF.Functions.Like(e.Venue.Name, $"%{location}%"));
        }

        if (searchDate.HasValue)
        {
            DateTime selectedDate = searchDate.Value.Date;

            query = query.Where(e =>
                e.EventDate.Date == selectedDate);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(e =>
                e.Price <= maxPrice.Value);
        }

        return query;
    }
    public async Task<IActionResult> Details(int id)
    {
        var ev = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Seats)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

        return View(ev);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(TicketPurchaseViewModel model)
    {
        if (model == null || model.Seats == null)
            return RedirectToAction("Details", new { id = model?.EventId });

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        decimal total = 0;
        int totalQuantity = 0;

        var seatIds = model.Seats.Select(s => s.SeatId).ToList();

        var eventSeats = await _context.EventSeats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();

        foreach (var selection in model.Seats)
        {
            if (selection.Quantity <= 0)
                continue;

            var seat = eventSeats.FirstOrDefault(s => s.Id == selection.SeatId);

            if (seat == null)
                continue;

            if (selection.Quantity > seat.Quantity)
            {
                TempData["Error"] = "Not enough seats available.";
                return RedirectToAction("Details", new { id = model.EventId });
            }

            total += selection.Quantity * seat.Price;
            totalQuantity += selection.Quantity;
        }

        if (total <= 0)
        {
            TempData["Error"] = "Please select at least one seat.";
            return RedirectToAction("Details", new { id = model.EventId });
        }

        // Simulate payment success
        bool paymentSuccess = true;

        if (!paymentSuccess)
        {
            TempData["Error"] = "Payment failed.";
            return RedirectToAction("Details", new { id = model.EventId });
        }

        // Deduct seats
        foreach (var selection in model.Seats)
        {
            if (selection.Quantity <= 0)
                continue;

            var seat = eventSeats.First(s => s.Id == selection.SeatId);
            seat.Quantity -= selection.Quantity;
        }

        // 🔥 CREATE TICKET RECORD
        var ticket = new Ticket
        {
            EventId = model.EventId,
            UserId = user.Id,
            Quantity = totalQuantity,
            TotalAmount = total,
            PurchaseDate = DateTime.Now
        };

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Payment Successful! Total Paid: ${total}";

        return RedirectToAction("MyTickets", "MemberDashboard");
    }

}
