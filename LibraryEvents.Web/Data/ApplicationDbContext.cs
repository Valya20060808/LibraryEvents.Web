using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryEvents.Web.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LibraryEvents.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Начальные данные для жанров
            builder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Научная литература", Color = "#6a11cb" },
                new Genre { Id = 2, Name = "Художественная литература", Color = "#2575fc" },
                new Genre { Id = 3, Name = "Детективы", Color = "#ff6b6b" },
                new Genre { Id = 4, Name = "Фантастика", Color = "#4cd97b" },
                new Genre { Id = 5, Name = "Исторические произведения", Color = "#ffc107" },
                new Genre { Id = 6, Name = "Биографии", Color = "#17a2b8" },
                new Genre { Id = 7, Name = "Поэзия", Color = "#e83e8c" },
                new Genre { Id = 8, Name = "Детская литература", Color = "#20c997" }
            );

            // Начальные данные для спикеров
            builder.Entity<Speaker>().HasData(
                new Speaker { Id = 1, Name = "Иванова Анна Петровна", Bio = "Доктор филологических наук, профессор МГУ" },
                new Speaker { Id = 2, Name = "Смирнов Алексей Владимирович", Bio = "Известный научный журналист, автор бестселлеров" },
                new Speaker { Id = 3, Name = "Петрова Екатерина Сергеевна", Bio = "Писатель-фантаст, лауреат премии 'Звездный мост'" },
                new Speaker { Id = 4, Name = "Козлов Дмитрий Игоревич", Bio = "Историк, специалист по средневековой литературе" }
            );

            // Начальные данные для мероприятий
            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Новые горизонты в научной фантастике",
                    Description = "Обсуждение современных тенденций в научной фантастике и их влияния на общество.",
                    Date = DateTime.Today.AddDays(7),
                    Time = new TimeSpan(18, 0, 0),
                    Location = "Конференц-зал №1",
                    GenreId = 4,
                    SpeakerId = 3
                },
                new Event
                {
                    Id = 2,
                    Title = "Исторические романы: правда и вымысел",
                    Description = "Лекция о балансе исторической достоверности и художественного вымысла в литературе.",
                    Date = DateTime.Today.AddDays(10),
                    Time = new TimeSpan(17, 30, 0),
                    Location = "Зал искусств",
                    GenreId = 5,
                    SpeakerId = 4
                },
                new Event
                {
                    Id = 3,
                    Title = "Современная поэзия: язык нового поколения",
                    Description = "Творческий вечер и дискуссия о современных поэтических формах.",
                    Date = DateTime.Today.AddDays(14),
                    Time = new TimeSpan(19, 0, 0),
                    Location = "Поэтический клуб",
                    GenreId = 7,
                    SpeakerId = 1
                }
            );

            // Конфигурация связей

            builder.Entity<Registration>()
              .HasOne(r => r.User)
               .WithMany()
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Event>()
                .HasOne(e => e.Genre)
                .WithMany()
                .HasForeignKey(e => e.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
                .HasOne(e => e.Speaker)
                .WithMany()
                .HasForeignKey(e => e.SpeakerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}