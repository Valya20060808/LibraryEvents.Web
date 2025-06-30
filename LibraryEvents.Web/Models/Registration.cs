using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryEvents.Web.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Пользователь обязателен")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        [ValidateNever] // Отключаем валидацию для навигационного свойства
        public ApplicationUser User { get; set; } = null!;

        [Required(ErrorMessage = "Мероприятие обязательно")]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        [ValidateNever] // Отключаем валидацию для навигационного свойства
        public Event Event { get; set; } = null!;

        [Required(ErrorMessage = "Дата регистрации обязательна")]
        [Display(Name = "Дата регистрации")]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Дополнительная информация")]
        [StringLength(500, ErrorMessage = "Максимальная длина 500 символов")]
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Подтверждена")]
        public bool IsConfirmed { get; set; } = true;

        [Display(Name = "Код подтверждения")]
        [StringLength(10, ErrorMessage = "Код должен быть не длиннее 10 символов")]
        public string? ConfirmationCode { get; set; }
    }
}