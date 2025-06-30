using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using LibraryEvents.Web.Data;
using LibraryEvents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryEvents.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int genreId = 0)
        {
            var events = _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Title.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            ViewData["CurrentGenre"] = genreId;
            ViewData["Genres"] = new SelectList(
                await _context.Genres.ToListAsync(),
                "Id",
                "Name",
                genreId  // Передаем выбранное значение
            );

            if (genreId != 0)
            {
                events = events.Where(e => e.GenreId == genreId);
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            // Проверка регистрации текущего пользователя и получение ID регистрации
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Получаем регистрацию, а не просто проверяем ее наличие
                var registration = await _context.Registrations
                    .FirstOrDefaultAsync(r => r.EventId == id && r.UserId == userId);

                ViewBag.IsRegistered = registration != null;
                ViewBag.RegistrationId = registration?.Id; // Сохраняем ID регистрации
            }
            else
            {
                ViewBag.IsRegistered = false;
                ViewBag.RegistrationId = null;
            }

            return View(@event);
        }

        // Остальные методы без изменений...
        // POST: Events/Register/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException();

            // Проверка, не зарегистрирован ли уже пользователь
            if (!await _context.Registrations.AnyAsync(r => r.EventId == id && r.UserId == userId))
            {
                _context.Registrations.Add(new Registration
                {
                    EventId = id,
                    UserId = userId,
                    RegisteredAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Events/MyRegistrations
        [Authorize]
        public async Task<IActionResult> MyRegistrations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var registrations = await _context.Registrations
                .Include(r => r.Event)
                .ThenInclude(e => e.Genre) // Добавляем жанр мероприятия
                .Include(r => r.Event.Speaker) // Добавляем спикера мероприятия
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Event.Date) // Сортировка по дате мероприятия
                .ToListAsync();

            return View(registrations);
        }

        // GET: Events/Create (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name");
            ViewData["Speakers"] = new SelectList(_context.Speakers, "Id", "Name");
            return View();
        }

        // POST: Events/Create (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ---- ОТЛАДОЧНЫЙ ВЫВОД ModelState ----
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var err in state.Errors)
                {
                    // Это будет видно в Output-окне Visual Studio!
                    Console.WriteLine($"[ModelState] {key}: {err.ErrorMessage}");
                }
            }
            // ---- КОНЕЦ ОТЛАДОЧНОГО ВЫВОДА ----

            // при ошибке повторно заполняем списки
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", @event.GenreId);
            ViewData["Speakers"] = new SelectList(_context.Speakers, "Id", "Name", @event.SpeakerId);
            return View(@event);
        }

        // GET: Events/Edit/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", @event.GenreId);
            ViewData["Speakers"] = new SelectList(_context.Speakers, "Id", "Name", @event.SpeakerId);
            return View(@event);
        }

        // POST: Events/Edit/5 (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == @event.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", @event.GenreId);
            ViewData["Speakers"] = new SelectList(_context.Speakers, "Id", "Name", @event.SpeakerId);
            return View(@event);
        }

        // GET: Events/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var @event = await _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null) return NotFound();
            return View(@event);
        }

        // POST: Events/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}