namespace consultorio_backend.Models
{
    public class Psychologist : Profile
    {
        public string LicenceNumber { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}
