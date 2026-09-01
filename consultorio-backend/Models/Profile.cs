using System.ComponentModel.DataAnnotations;

namespace consultorio_backend.Models
{
    public abstract class Profile : Entity
    {
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        [Required]
        public string DNI { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
