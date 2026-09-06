using consultorio_backend.Models;
using consultorio_backend.Models.Enums;
using consultorio_backend.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace consultorio_backend.Data.Seeders
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DatabaseSeeder(
            AppDbContext context, 
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();

            if (!_context.Users.Any())
            {
                await SeedUsersAsync();
                await SeedPsychologistsAsync();
                await SeedPatientsAsync();
            }

            if (!_context.Appointments.Any())
            {
                await SeedAppointmentsAsync();
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in UserRoles.All)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            var adminUser = new AppUser
            {
                Email = "admin@consultorio.com",
                UserName = "admin@consultorio.com",
                FirstName = "Admin",
                LastName = "Sistema",
                EmailConfirmed = true
            };

            var garciaUser = new AppUser
            {
                Email = "psic.garcia@consultorio.com",
                UserName = "psic.garcia@consultorio.com",
                FirstName = "Carlos",
                LastName = "García",
                EmailConfirmed = true
            };

            var perezUser = new AppUser
            {
                Email = "juan.perez@example.com",
                UserName = "juan.perez@example.com",
                FirstName = "Juan",
                LastName = "Pérez",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, "Password@123456");
            await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

            await _userManager.CreateAsync(garciaUser, "Password@123456");
            await _userManager.AddToRoleAsync(garciaUser, UserRoles.Psychologist);

            await _userManager.CreateAsync(perezUser, "Password@123456");
            await _userManager.AddToRoleAsync(perezUser, UserRoles.Patient);
        }

        private async Task SeedPsychologistsAsync()
        {
            var garciUser = _context.Users.First(u => u.Email == "psic.garcia@consultorio.com");

            var psychologists = new List<Psychologist>
            {
                new Psychologist
                {
                    FirstName = "Carlos",
                    LastName = "García",
                    SecondLastName = "Rodríguez",
                    BirthDate = new DateTime(1980, 5, 15),
                    PhoneNumber = "555-0001",
                    Email = "psic.garcia@consultorio.com",
                    LicenceNumber = "PSI-2024-001",
                    Specialty = "Psicología Clínica",
                    AppUserId = garciUser.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Psychologist
                {
                    FirstName = "María",
                    LastName = "López",
                    SecondLastName = "Gómez",
                    BirthDate = new DateTime(1985, 3, 22),
                    PhoneNumber = "555-0002",
                    Email = "psic.lopez@consultorio.com",
                    LicenceNumber = "PSI-2024-002",
                    Specialty = "Psicología Infantil",
                    CreatedAt = DateTime.UtcNow
                },
                new Psychologist
                {
                    FirstName = "Javier",
                    LastName = "Martínez",
                    SecondLastName = "Sánchez",
                    BirthDate = new DateTime(1978, 11, 5),
                    PhoneNumber = "555-0003",
                    Email = "psic.martinez@consultorio.com",
                    LicenceNumber = "PSI-2024-003",
                    Specialty = "Psicología Educativa",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Psychologists.AddRangeAsync(psychologists);
            await _context.SaveChangesAsync();
        }

        private async Task SeedPatientsAsync()
        {
            var perezUser = _context.Users.First(u => u.Email == "juan.perez@example.com");

            var patients = new List<Patient>
            {
                new Patient
                {
                    FirstName = "Juan",
                    LastName = "Pérez",
                    SecondLastName = "González",
                    BirthDate = new DateTime(1990, 7, 10),
                    PhoneNumber = "555-5001",
                    Email = "juan.perez@example.com",
                    AppUserId = perezUser.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Ana",
                    LastName = "Martínez",
                    SecondLastName = "Sánchez",
                    BirthDate = new DateTime(1988, 2, 14),
                    PhoneNumber = "555-5002",
                    Email = "ana.martinez@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Pedro",
                    LastName = "Fernández",
                    SecondLastName = "López",
                    BirthDate = new DateTime(1992, 11, 25),
                    PhoneNumber = "555-5003",
                    Email = "pedro.fernandez@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Laura",
                    LastName = "García",
                    SecondLastName = "Díaz",
                    BirthDate = new DateTime(1995, 6, 8),
                    PhoneNumber = "555-5004",
                    Email = "laura.garcia@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Diego",
                    LastName = "Rodríguez",
                    SecondLastName = "Vargas",
                    BirthDate = new DateTime(1987, 9, 3),
                    PhoneNumber = "555-5005",
                    Email = "diego.rodriguez@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAppointmentsAsync()
        {
            var garcia = _context.Psychologists.First(p => p.Email == "psic.garcia@consultorio.com");

            var juan = _context.Patients.First(p => p.Email == "juan.perez@example.com");
            var ana = _context.Patients.First(p => p.Email == "ana.martinez@example.com");
            var pedro = _context.Patients.First(p => p.Email == "pedro.fernandez@example.com");
            var laura = _context.Patients.First(p => p.Email == "laura.garcia@example.com");
            var diego = _context.Patients.First(p => p.Email == "diego.rodriguez@example.com");

            var now = DateTime.UtcNow;

            var appointments = new List<Appointment>
            {
                // Historial de Juan Pérez: variado en status, type y tiempo (para nutrir su dashboard)
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-30).Date.AddHours(10),
                    EndAt = now.AddDays(-30).Date.AddHours(11),
                    Status = AppointmentStatus.Completed,
                    Type = AppointmentType.InPerson,
                    Notes = "Primera sesión de evaluación."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-21).Date.AddHours(10),
                    EndAt = now.AddDays(-21).Date.AddHours(11),
                    Status = AppointmentStatus.Completed,
                    Type = AppointmentType.Online,
                    Notes = "Seguimiento de terapia cognitivo-conductual."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-14).Date.AddHours(16),
                    EndAt = now.AddDays(-14).Date.AddHours(17),
                    Status = AppointmentStatus.NoShow,
                    Type = AppointmentType.InPerson,
                    Notes = "El paciente no asistió."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-7).Date.AddHours(9),
                    EndAt = now.AddDays(-7).Date.AddHours(10),
                    Status = AppointmentStatus.Cancelled,
                    Type = AppointmentType.Online,
                    Notes = "Cancelada por el paciente con anticipación."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-2).Date.AddHours(15),
                    EndAt = now.AddDays(-2).Date.AddHours(16),
                    Status = AppointmentStatus.Completed,
                    Type = AppointmentType.InPerson,
                    Notes = "Buen progreso en las técnicas de manejo de ansiedad."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(3).Date.AddHours(11),
                    EndAt = now.AddDays(3).Date.AddHours(12),
                    Status = AppointmentStatus.Confirmed,
                    Type = AppointmentType.Online,
                    Notes = "Sesión de seguimiento programada."
                },
                new Appointment
                {
                    PatientId = juan.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(10).Date.AddHours(9),
                    EndAt = now.AddDays(10).Date.AddHours(10),
                    Status = AppointmentStatus.Scheduled,
                    Type = AppointmentType.InPerson
                },

                // Citas de otros pacientes, para variedad general en el sistema
                new Appointment
                {
                    PatientId = ana.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-10).Date.AddHours(12),
                    EndAt = now.AddDays(-10).Date.AddHours(13),
                    Status = AppointmentStatus.Completed,
                    Type = AppointmentType.InPerson,
                    Notes = "Consulta inicial."
                },
                new Appointment
                {
                    PatientId = pedro.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(-5).Date.AddHours(14),
                    EndAt = now.AddDays(-5).Date.AddHours(15),
                    Status = AppointmentStatus.NoShow,
                    Type = AppointmentType.Online
                },
                new Appointment
                {
                    PatientId = laura.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(5).Date.AddHours(10),
                    EndAt = now.AddDays(5).Date.AddHours(11),
                    Status = AppointmentStatus.Scheduled,
                    Type = AppointmentType.InPerson
                },
                new Appointment
                {
                    PatientId = diego.Id,
                    PsychologistId = garcia.Id,
                    StartAt = now.AddDays(15).Date.AddHours(17),
                    EndAt = now.AddDays(15).Date.AddHours(18),
                    Status = AppointmentStatus.Confirmed,
                    Type = AppointmentType.Online
                }
            };

            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();
        }
    }
}
