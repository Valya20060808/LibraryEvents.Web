using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryEvents.Web.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Название события' обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "Название события не может превышать 200 символов")]
        [Display(Name = "Название события")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Описание' обязательно для заполнения")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Дата мероприятия' обязательно для заполнения")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата мероприятия")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Поле 'Время начала' обязательно для заполнения")]
        [DataType(DataType.Time)]
        [Display(Name = "Время начала")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "Поле 'Место проведения' обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "Место проведения не может превышать 200 символов")]
        [Display(Name = "Место проведения")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Жанр' обязательно для заполнения")]
        [Display(Name = "Жанр")]
        public int GenreId { get; set; }

        [ValidateNever]
        [ForeignKey("GenreId")]
        public Genre Genre { get; set; } = null!;

        [Required(ErrorMessage = "Поле 'Спикер' обязательно для заполнения")]
        [Display(Name = "Спикер")]
        public int SpeakerId { get; set; }

        [ValidateNever]
        [ForeignKey("SpeakerId")]
        public Speaker Speaker { get; set; } = null!;

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        [NotMapped]
        public DateTime StartDateTime => Date.Add(Time);

        [NotMapped]
        public DateTime EndDateTime => Date.Add(Time).AddHours(1);

        [NotMapped]
        public string EventColor => Genre?.Color ?? "#6a11cb";
    }
}