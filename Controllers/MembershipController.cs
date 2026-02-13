using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

[Authorize]
public class MembershipController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;

    public MembershipController(
        SmartEventManagement_TicketingSystemContext context,
        UserManager<SmartEventManagement_TicketingSystemUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ GET: Payment page (NO antiforgery here)
    [HttpGet]
    public async Task<IActionResult> Pay()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (user.IsMember)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // ✅ POST: Confirm payment (antiforgery REQUIRED)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayConfirm()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (user.IsMember)
            return RedirectToAction("Index", "Home");

        var payment = new MembershipPayment
        {
            UserId = user.Id,
            Amount = 29.00m,
            PaymentDate = DateTime.UtcNow,
            Status = "Success"
        };

        _context.MembershipPayments.Add(payment);

        user.IsMember = true;
        await _userManager.UpdateAsync(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success()
    {
        return View();
    }
}
