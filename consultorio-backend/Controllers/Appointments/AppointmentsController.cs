using System.Security.Claims;
using consultorio_backend.Data;
using consultorio_backend.DTOs.Appointments;
using consultorio_backend.Models;
using consultorio_backend.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace consultorio_backend.Controllers.Appointments
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las citas. Solo Admin y Psychologist.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Psychologist)
                .Select(a => ToResponse(a))
                .ToListAsync();

            return Ok(appointments);
        }

        /// <summary>
        /// Obtiene las citas del usuario autenticado: si es Patient, sus propias citas;
        /// si es Psychologist, las citas que él atiende.
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = $"{UserRoles.Patient},{UserRoles.Psychologist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetMyAppointments()
        {
            var appUserId = GetCurrentAppUserId();
            if (appUserId == null)
                return Unauthorized();

            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Psychologist)
                .AsQueryable();

            query = User.IsInRole(UserRoles.Psychologist)
                ? query.Where(a => a.Psychologist.AppUserId == appUserId)
                : query.Where(a => a.Patient.AppUserId == appUserId);

            var appointments = await query
                .Select(a => ToResponse(a))
                .ToListAsync();

            return Ok(appointments);
        }

        /// <summary>
        /// Obtiene una cita por Id. Admin/Psychologist pueden ver cualquiera;
        /// Patient solo puede ver la suya.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Psychologist)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            if (User.IsInRole(UserRoles.Patient) && !User.IsInRole(UserRoles.Admin) && !User.IsInRole(UserRoles.Psychologist))
            {
                var appUserId = GetCurrentAppUserId();
                if (appointment.Patient.AppUserId != appUserId)
                    return Forbid();
            }

            return Ok(ToResponse(appointment));
        }

        /// <summary>
        /// Crea una nueva cita.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<AppointmentResponse>> CreateAppointment([FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId);
            if (!patientExists)
                return BadRequest(new { message = "El paciente indicado no existe" });

            var psychologistExists = await _context.Psychologists.AnyAsync(p => p.Id == request.PsychologistId);
            if (!psychologistExists)
                return BadRequest(new { message = "El psicólogo indicado no existe" });

            var appointment = new Appointment
            {
                PatientId = request.PatientId,
                PsychologistId = request.PsychologistId,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Status = request.Status,
                Type = request.Type,
                Notes = request.Notes
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Psychologist).LoadAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, ToResponse(appointment));
        }

        /// <summary>
        /// Actualiza una cita existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<AppointmentResponse>> UpdateAppointment(int id, [FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            appointment.PatientId = request.PatientId;
            appointment.PsychologistId = request.PsychologistId;
            appointment.StartAt = request.StartAt;
            appointment.EndAt = request.EndAt;
            appointment.Status = request.Status;
            appointment.Type = request.Type;
            appointment.Notes = request.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Psychologist).LoadAsync();

            return Ok(ToResponse(appointment));
        }

        /// <summary>
        /// Elimina una cita.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Cita no encontrada" });

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetCurrentAppUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        private static AppointmentResponse ToResponse(Appointment appointment) => new()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
            PsychologistId = appointment.PsychologistId,
            PsychologistName = $"{appointment.Psychologist.FirstName} {appointment.Psychologist.LastName}",
            StartAt = appointment.StartAt,
            EndAt = appointment.EndAt,
            Status = appointment.Status,
            Type = appointment.Type,
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
}
