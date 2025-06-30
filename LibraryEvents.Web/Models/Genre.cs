using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEvents.Web.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название жанра обязательно")]
        [StringLength(100, ErrorMessage = "Название жанра не должно превышать 100 символов")]
        [Display(Name = "Название жанра")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цвет обязателен")]
        [StringLength(7, ErrorMessage = "Цвет должен быть в HEX-формате (#FFFFFF)")]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Неверный формат цвета. Используйте HEX-формат (#FFFFFF)")]
        [Display(Name = "Цвет жанра")]
        public string Color { get; set; } = "#6a11cb"; // Фиолетовый по умолчанию

        [NotMapped]
        [Display(Name = "Образец цвета")]
        public string ColorSample => $"<div style=\"width:20px;height:20px;background:{Color};border-radius:50%;\"></div>";
    }
}