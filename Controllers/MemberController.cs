using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using SmartEventManagement_TicketingSystem.Models;

[Authorize(Roles = "Member")]
public class MemberController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;

    public MemberController(
        SmartEventManagement_TicketingSystemContext context,
        UserManager<SmartEventManagement_TicketingSystemUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // =====================================
    // MY TICKETS
    // =====================================
    public async Task<IActionResult> MyTickets()
    {
        var userId = _userManager.GetUserId(User);

        var tickets = await _context.Tickets
            .Include(t => t.Event)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return View("~/Views/MemberDashboard/MyTickets.cshtml", tickets);
    }


    // =====================================
    // SUBMIT FEEDBACK
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitFeedback(int eventId, int rating)
    {
        var userId = _userManager.GetUserId(User);

        // Ensure user bought ticket
        var hasTicket = await _context.Tickets
            .AnyAsync(t => t.EventId == eventId && t.UserId == userId);

        if (!hasTicket)
        {
            TempData["Error"] = "You cannot rate an event you didn't attend.";
            return RedirectToAction("MyTickets");
        }

        // Prevent duplicate feedback
        var existing = await _context.Feedbacks
            .FirstOrDefaultAsync(f =>
                f.EventId == eventId &&
                f.UserId == userId);

        if (existing != null)
        {
            TempData["Error"] = "You already submitted feedback.";
            return RedirectToAction("MyTickets");
        }

        var feedback = new Feedback
        {
            EventId = eventId,
            UserId = userId,
            Rating = rating
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thank you for your feedback!";

        return RedirectToAction("MyTickets");
    }
}
