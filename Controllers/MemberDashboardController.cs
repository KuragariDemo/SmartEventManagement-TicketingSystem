using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

[Authorize]
public class MemberDashboardController : Controller
{
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;

    public MemberDashboardController(
        UserManager<SmartEventManagement_TicketingSystemUser> userManager)
    {
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
}
