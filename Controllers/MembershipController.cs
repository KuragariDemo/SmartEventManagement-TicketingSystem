using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using System;
using System.Threading.Tasks;

[Authorize]
public class MembershipController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;
    private readonly SignInManager<SmartEventManagement_TicketingSystemUser> _signInManager;

    public MembershipController(
        SmartEventManagement_TicketingSystemContext context,
        UserManager<SmartEventManagement_TicketingSystemUser> userManager,
        SignInManager<SmartEventManagement_TicketingSystemUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // ======================================
    // GET: Membership/Pay
    // ======================================
    [HttpGet]
    public async Task<IActionResult> Pay()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        // Admin does not need membership payment
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index", "AdminDashboard");

        // Already a member
        if (user.IsMember)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // ======================================
    // POST: Membership/PayConfirm
    // ======================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayConfirm()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        // Admin skip
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index", "AdminDashboard");

        // Prevent double payment
        if (user.IsMember)
            return RedirectToAction("Index", "Home");

        // Create payment record
        var payment = new MembershipPayment
        {
            UserId = user.Id,
            Amount = 29.00m,
            PaymentDate = DateTime.UtcNow,
            Status = "Success"
        };

        _context.MembershipPayments.Add(payment);

        // Update user membership status
        user.IsMember = true;
        await _userManager.UpdateAsync(user);

        // Add Member role if not already assigned
        if (!await _userManager.IsInRoleAsync(user, "Member"))
        {
            await _userManager.AddToRoleAsync(user, "Member");
        }

        // 🔥 Refresh login cookie (FIX logout problem)
        await _signInManager.RefreshSignInAsync(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Success));
    }

    // ======================================
    // GET: Membership/Success
    // ======================================
    [HttpGet]
    public async Task<IActionResult> Success()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Index", "AdminDashboard");

        if (!user.IsMember)
            return RedirectToAction(nameof(Pay));

        return View();
    }
}
