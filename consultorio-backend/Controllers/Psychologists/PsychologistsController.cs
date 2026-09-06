using System.Security.Claims;
using consultorio_backend.Data;
using consultorio_backend.DTOs.Psychologists;
using consultorio_backend.Models;
using consultorio_backend.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace consultorio_backend.Controllers.Psychologists
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PsychologistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PsychologistsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los psicólogos. Solo Admin (usado en el formulario de citas).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<PsychologistResponse>>> GetPsychologists()
        {
            var psychologists = await _context.Psychologists
                .Select(p => ToResponse(p))
                .ToListAsync();

            return Ok(psychologists);
        }

        /// <summary>
        /// Obtiene los datos del psicólogo autenticado (para su dashboard).
        /// </summary>
        [HttpGet("me")]
        [Authorize(Roles = UserRoles.Psychologist)]
        public async Task<ActionResult<PsychologistResponse>> GetMe()
        {
            var appUserId = GetCurrentAppUserId();
            if (appUserId == null)
                return Unauthorized();

            var psychologist = await _context.Psychologists
                .FirstOrDefaultAsync(p => p.AppUserId == appUserId);

            if (psychologist == null)
                return NotFound(new { message = "Perfil de psicólogo no encontrado" });

            return Ok(ToResponse(psychologist));
        }

        private int? GetCurrentAppUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        private static PsychologistResponse ToResponse(Psychologist psychologist) => new()
        {
            Id = psychologist.Id,
            FirstName = psychologist.FirstName,
            MiddleName = psychologist.MiddleName,
            LastName = psychologist.LastName,
            SecondLastName = psychologist.SecondLastName,
            BirthDate = psychologist.BirthDate,
            PhoneNumber = psychologist.PhoneNumber,
            Email = psychologist.Email,
            LicenceNumber = psychologist.LicenceNumber,
            Specialty = psychologist.Specialty,
            AppUserId = psychologist.AppUserId,
            CreatedAt = psychologist.CreatedAt,
            UpdatedAt = psychologist.UpdatedAt
        };
    }
}
