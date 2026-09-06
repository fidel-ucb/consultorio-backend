namespace consultorio_backend.DTOs.Psychologists
{
    public class PsychologistResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string LicenceNumber { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int? AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
