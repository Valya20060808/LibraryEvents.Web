using LibraryEvents.Web.Data;
using LibraryEvents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryEvents.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
           
            // Создаем роли
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Создаем admin пользователя
            const string adminEmail = "admin@library.com";
            const string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // убедитесь, что роль создана заранее:
                    if (!await roleManager.RoleExistsAsync("Admin"))
                        await roleManager.CreateAsync(new IdentityRole("Admin"));

                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }


            // Добавляем начальные жанры и спикеров
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Name = "Лекция" },
                    new Genre { Name = "Мастер-класс" },
                    new Genre { Name = "Встреча с автором" }
                );
            }
            if (!context.Speakers.Any())
            {
                context.Speakers.AddRange(
                    new Speaker { Name = "Иван Иванов", Bio = "Библиотекарь с 10-летним стажем." },
                    new Speaker { Name = "Мария Петрова", Bio = "Автор и лектор." }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}