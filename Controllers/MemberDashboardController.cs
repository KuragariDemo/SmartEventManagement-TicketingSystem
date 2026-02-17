using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using SmartEventManagement_TicketingSystem.Data;


[Authorize]
public class MemberDashboardController : Controller
{
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;
    private readonly SmartEventManagement_TicketingSystemContext _context;
    public MemberDashboardController(
    SmartEventManagement_TicketingSystemContext context,
    UserManager<SmartEventManagement_TicketingSystemUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        if (!user.IsMember)
            return RedirectToAction("Pay", "Membership");

        return View(user);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        return View(user);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(
    SmartEventManagement_TicketingSystemUser model,
    string[] preferences)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;
        user.City = model.City;
        user.Bio = model.Bio;

        // Save preferences as comma separated string
        user.EventPreferences = string.Join(",", preferences);

        await _userManager.UpdateAsync(user);

        ViewBag.Success = "Profile updated successfully!";
        return View(user);
    }


    public IActionResult Events()
    {
        return View();
    }

    public IActionResult Upcoming()
    {
        return View();
    }

    public IActionResult Booked()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> MyTickets()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var tickets = await _context.Tickets
            .Include(t => t.Event)
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.Event.EventDate)
            .ToListAsync();

        return View(tickets);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitFeedback(int eventId, int rating)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        // Validate rating range
        if (rating < 1 || rating > 5)
        {
            TempData["Error"] = "Invalid rating value.";
            return RedirectToAction("MyTickets");
        }

        // Check if user purchased ticket
        var hasTicket = await _context.Tickets
            .AnyAsync(t => t.EventId == eventId && t.UserId == user.Id);

        if (!hasTicket)
        {
            TempData["Error"] = "You cannot rate an event you didn't attend.";
            return RedirectToAction("MyTickets");
        }

        // Prevent duplicate feedback
        var existing = await _context.Feedbacks
            .FirstOrDefaultAsync(f =>
                f.EventId == eventId &&
                f.UserId == user.Id);

        if (existing != null)
        {
            TempData["Error"] = "You already submitted feedback.";
            return RedirectToAction("MyTickets");
        }

        var feedback = new Feedback
        {
            EventId = eventId,
            UserId = user.Id,
            Rating = rating
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thank you for your feedback!";
        return RedirectToAction("MyTickets");
    }

}
