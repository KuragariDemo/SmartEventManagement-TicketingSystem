using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEventManagement_TicketingSystem.Data;
using SmartEventManagement_TicketingSystem.Models;
using SmartEventManagement_TicketingSystem.Models.Events;

namespace SmartEventManagement_TicketingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SmartEventManagement_TicketingSystemContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            SmartEventManagement_TicketingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestEvents = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.EventDate >= DateTime.Now)
                .OrderByDescending(e => e.EventDate)
                .Take(3)
                .ToListAsync();

            return View(latestEvents);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
