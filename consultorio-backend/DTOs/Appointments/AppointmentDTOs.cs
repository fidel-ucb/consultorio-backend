using consultorio_backend.Models.Enums;

namespace consultorio_backend.DTOs.Appointments
{
    public class AppointmentRequest
    {
        public int PatientId { get; set; }
        public int PsychologistId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentResponse
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
