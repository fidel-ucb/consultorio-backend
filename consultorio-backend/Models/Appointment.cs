using System.ComponentModel.DataAnnotations;
using consultorio_backend.Models.Enums;

namespace consultorio_backend.Models
{
    public class Appointment : Entity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public AppointmentType Type { get; set; }

        public string? Notes { get; set; }
    }
}
