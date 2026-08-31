using consultorio_backend.Models.Enums;
using System.ComponentModel.DataAnnotations;
 
namespace consultorio_backend.Models
{
    public class User : Entity
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public UserRole Role { get; set; }
    }
}
