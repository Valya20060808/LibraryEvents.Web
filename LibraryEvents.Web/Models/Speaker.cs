using System.ComponentModel.DataAnnotations;

namespace LibraryEvents.Web.Models
{
    public class Speaker
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Bio может быть пустым
        public string? Bio { get; set; }
    }
}