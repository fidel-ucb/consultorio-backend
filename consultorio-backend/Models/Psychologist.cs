using System.ComponentModel.DataAnnotations;

namespace consultorio_backend.Models
{
    public class Psychologist : Profile
    {
        [Required]
        public string LicenceNumber { get; set; } = string.Empty;

        [Required]
        public string Specialty { get; set; } = string.Empty;
    }
}
