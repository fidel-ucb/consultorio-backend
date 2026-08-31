using System.ComponentModel.DataAnnotations;

namespace consultorio_backend.Models
{
    public class Profile : Entity
    {
        public int? UserId { get; set; }
        public User? User { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
