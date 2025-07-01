using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LibraryEvents.Web.Data;
using LibraryEvents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryEvents.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Time)
                .Where(e => e.Date >= DateTime.Today)
                .Take(4)
                .ToListAsync();

            return View(events);
        }

        // Добавьте этот метод
        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}