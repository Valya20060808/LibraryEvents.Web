using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryEvents.Web.Data;
using LibraryEvents.Web.Models;

namespace LibraryEvents.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем список жанров для фильтрации
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker) // Добавляем загрузку спикера
                .Select(e => new
                {
                    id = e.Id,
                    title = e.Title,
                    start = e.Date.Add(e.Time).ToString("o"), // Формат ISO 8601
                    url = Url.Action("Details", "Events", new { id = e.Id }),
                    color = e.Genre != null ? e.Genre.Color : "#6a11cb",
                    extendedProps = new
                    {
                        location = e.Location,
                        speaker = e.Speaker != null ? e.Speaker.Name : "",
                        genreId = e.GenreId // Ключевое добавление для фильтрации
                    }
                })
                .ToListAsync();

            return Json(events);
        }
    }
}