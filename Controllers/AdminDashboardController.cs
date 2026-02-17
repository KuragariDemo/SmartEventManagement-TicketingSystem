using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Models.Events;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly SmartEventManagement_TicketingSystemContext _context;
    private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public AdminDashboardController(
        SmartEventManagement_TicketingSystemContext context,
        UserManager<SmartEventManagement_TicketingSystemUser> userManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    // =========================
    // DASHBOARD
    // =========================
    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            Categories = await _context.Categories
                .Select(c => c.Name)
                .ToListAsync(),

            Locations = await _context.Venues
                .Select(v => v.Name)
                .ToListAsync(),

            UpcomingEvents = events
                .Where(e => e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .ToList(),

            FinishedEvents = events
                .Where(e => e.EventDate < DateTime.Now)
                .OrderByDescending(e => e.EventDate)
                .ToList()
        };

        return View(model);
    }

    // =========================
    // CATEGORY CRUD
    // =========================

    public async Task<IActionResult> AddCategory()
    {
        return View(await _context.Categories.ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(Category model)
    {
        if (!ModelState.IsValid)
            return View(await _context.Categories.ToListAsync());

        _context.Categories.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(AddCategory));
    }

    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, Category model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(AddCategory));
    }

    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(AddCategory));
    }

    // =========================
    // VENUE CRUD
    // =========================

    public async Task<IActionResult> AddVenue()
    {
        return View(await _context.Venues.ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVenue(Venue model)
    {
        if (!ModelState.IsValid)
            return View(await _context.Venues.ToListAsync());

        _context.Venues.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(AddVenue));
    }

    public async Task<IActionResult> EditVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null) return NotFound();
        return View(venue);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditVenue(int id, Venue model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(AddVenue));
    }

    public async Task<IActionResult> DeleteVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue != null)
        {
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(AddVenue));
    }

    // =========================
    // EVENT CRUD
    // =========================

    public async Task<IActionResult> CreateEvent()
    {
        ViewBag.Venues = await _context.Venues.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(await _context.Events
    .Include(e => e.Category)
    .Include(e => e.Venue)
    .Include(e => e.Seats)
    .OrderByDescending(e => e.EventDate)
    .ToListAsync());

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
    Event model,
    int EgoQuantity, decimal EgoPrice,
    int VipQuantity, decimal VipPrice,
    int VvipQuantity, decimal VvipPrice)
    {
        ViewBag.Venues = await _context.Venues.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();

        if (!ModelState.IsValid)
            return View(await _context.Events.ToListAsync());

        // 🔥 IMAGE UPLOAD FIX
        if (model.ImageFile != null)
        {
            string uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "images",
                "events"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(model.ImageFile.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(fileStream);
            }

            model.ImageUrl = fileName; 
        }

        _context.Events.Add(model);
        await _context.SaveChangesAsync();

        // 🔥 ADD SEATS
        var seats = new List<EventSeat>();

        if (EgoQuantity > 0)
        {
            seats.Add(new EventSeat
            {
                EventId = model.Id,
                SeatType = "EGO",
                Quantity = EgoQuantity,
                Price = EgoPrice
            });
        }

        if (VipQuantity > 0)
        {
            seats.Add(new EventSeat
            {
                EventId = model.Id,
                SeatType = "VIP",
                Quantity = VipQuantity,
                Price = VipPrice
            });
        }

        if (VvipQuantity > 0)
        {
            seats.Add(new EventSeat
            {
                EventId = model.Id,
                SeatType = "VVIP",
                Quantity = VvipQuantity,
                Price = VvipPrice
            });
        }

        if (seats.Any())
        {
            _context.EventSeats.AddRange(seats);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(CreateEvent));
    }


    public async Task<IActionResult> EditEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        ViewBag.Venues = await _context.Venues.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(ev);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(int id, Event model)
    {
        if (id != model.Id) return NotFound();

        var existing = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return NotFound();

        if (model.ImageFile != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/events");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + "_" + model.ImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            model.ImageUrl = uniqueFileName;
        }
        else
        {
            model.ImageUrl = existing.ImageUrl;
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(CreateEvent));
    }

    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev != null)
        {
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(CreateEvent));
    }

    //ADD Admin
    public IActionResult AddAdmin()
    {
        var admins = _userManager.Users.ToList();
        return View(admins);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAdmin(
    string fullName,
    string email,
    string password,
    string city)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Email and Password are required.");
        }

        if (ModelState.IsValid)
        {
            var user = new SmartEventManagement_TicketingSystemUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                City = city,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AddAdmin));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        var admins = _userManager.Users.ToList();
        return View(admins);
    }

    public async Task<IActionResult> DeleteAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction(nameof(AddAdmin));
    }

}

