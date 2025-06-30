using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryEvents.Web.Data;
using LibraryEvents.Web.Models;

namespace LibraryEvents.Web.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistrationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Личный кабинет пользователя
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var registrations = await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Genre)
                .Include(r => r.Event)  // Добавлено для загрузки спикера
                    .ThenInclude(e => e.Speaker) // Загружаем данные спикера
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.Event.Date)
                .ToListAsync();

            return View(registrations);
        }

        // ... остальные методы без изменений ...
        // Форма регистрации на мероприятие
        [HttpGet]
        public async Task<IActionResult> Register(int eventId)
        {
            var eventItem = await _context.Events
                .Include(e => e.Genre)
                .Include(e => e.Speaker) // Рекомендуется добавить и здесь
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Проверка, не зарегистрирован ли уже
            var user = await _userManager.GetUserAsync(User);
            var existingRegistration = await _context.Registrations
                .AnyAsync(r => r.EventId == eventId && r.UserId == user.Id);

            if (existingRegistration)
            {
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            ViewBag.Event = eventItem;
            return View(new Registration { EventId = eventId });
        }

        // Обработка регистрации
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration registration)
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Перенаправит на страницу входа
            }

            // Устанавливаем UserId сразу
            registration.UserId = user.Id;

            var eventItem = await _context.Events
                .Include(e => e.Speaker) // Рекомендуется добавить
                .FirstOrDefaultAsync(e => e.Id == registration.EventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Проверка дублирования
            var existingRegistration = await _context.Registrations
                .AnyAsync(r => r.EventId == registration.EventId && r.UserId == user.Id);

            if (existingRegistration)
            {
                ModelState.AddModelError("", "Вы уже зарегистрированы на это мероприятие");
            }

            // Проверка чекбокса согласия
            var agreeTerms = Request.Form["agreeTerms"].FirstOrDefault();
            if (agreeTerms != "on")
            {
                ModelState.AddModelError("", "Вы должны согласиться с правилами участия");
            }

            if (ModelState.IsValid)
            {
                registration.RegisteredAt = DateTime.Now;
                registration.IsConfirmed = true;

                _context.Registrations.Add(registration);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = registration.Id });
            }

            ViewBag.Event = eventItem;
            return View(registration);
        }

        // Подтверждение регистрации
        public async Task<IActionResult> Confirmation(int id)
        {
            var registration = await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Speaker) // Добавлено для загрузки спикера
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // Отмена регистрации
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            // Проверка, что пользователь отменяет свою регистрацию
            var user = await _userManager.GetUserAsync(User);
            if (registration.UserId != user.Id)
            {
                return Forbid();
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}