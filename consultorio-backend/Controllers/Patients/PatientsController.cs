using consultorio_backend.Data;
using consultorio_backend.DTOs.Patients;
using consultorio_backend.DTOs.Psychologists;
using consultorio_backend.Models;
using consultorio_backend.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace consultorio_backend.Controllers.Patients
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los pacientes.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<IEnumerable<PatientResponse>>> GetPatients()
        {
            var patients = await _context.Patients
                .Select(p => ToResponse(p))
                .ToListAsync();

            return Ok(patients);
        }

        /// <summary>
        /// Obtiene un paciente por Id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<PatientResponse>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Paciente no encontrado" });

            return Ok(ToResponse(patient));
        }

        /// <summary>
        /// Obtiene los datos del paciente autenticado (para su dashboard).
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = UserRoles.Patient)]
        public async Task<ActionResult<PatientResponse>> GetMe()
        {
            var appUserId = GetCurrentAppUserId();
            if (appUserId == null)
                return Unauthorized();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.AppUserId == appUserId);

            if (patient == null)
                return NotFound(new { message = "Perfil de paciente no encontrado" });

            return Ok(ToResponse(patient));
        }

        /// <summary>
        /// Crea un nuevo paciente.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<PatientResponse>> CreatePatient([FromBody] PatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = new Patient
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                SecondLastName = request.SecondLastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                AppUserId = request.AppUserId
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, ToResponse(patient));
        }

        /// <summary>
        /// Actualiza un paciente existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Psychologist}")]
        public async Task<ActionResult<PatientResponse>> UpdatePatient(int id, [FromBody] PatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Paciente no encontrado" });

            patient.FirstName = request.FirstName;
            patient.MiddleName = request.MiddleName;
            patient.LastName = request.LastName;
            patient.SecondLastName = request.SecondLastName;
            patient.BirthDate = request.BirthDate;
            patient.PhoneNumber = request.PhoneNumber;
            patient.Email = request.Email;
            patient.AppUserId = request.AppUserId;
            patient.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ToResponse(patient));
        }

        /// <summary>
        /// Elimina un paciente.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Paciente no encontrado" });

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetCurrentAppUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        private static PatientResponse ToResponse(Patient patient) => new()
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            MiddleName = patient.MiddleName,
            LastName = patient.LastName,
            SecondLastName = patient.SecondLastName,
            BirthDate = patient.BirthDate,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            AppUserId = patient.AppUserId,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt
        };
    }
}
