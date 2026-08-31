using consultorio_backend.Models.Enums;

namespace consultorio_backend.Models
{
    public class Appointment : Entity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
