using consultorio_backend.Models;
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

            // Verificar si ya hay usuarios
            if (_context.Users.Any())
                return;

            await SeedUsersAsync();
            await SeedPsychologistsAsync();
            await SeedPatientsAsync();

            await _context.SaveChangesAsync();
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
                Email = "doctor.garcia@consultorio.com",
                UserName = "doctor.garcia@consultorio.com",
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
            var garciUser = _context.Users.First(u => u.Email == "doctor.garcia@consultorio.com");

            var psychologists = new List<Psychologist>
            {
                new Psychologist
                {
                    FirstName = "Carlos",
                    LastName = "García",
                    SecondLastName = "Rodríguez",
                    DNI = "12345678",
                    BirthDate = new DateTime(1980, 5, 15),
                    PhoneNumber = "555-0001",
                    Email = "doctor.garcia@consultorio.com",
                    LicenceNumber = "PSI-2024-001",
                    Specialty = "Psicología Clínica",
                    AppUserId = garciUser.Id,
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
                    DNI = "11223344",
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
                    DNI = "22334455",
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
                    DNI = "33445566",
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
                    DNI = "44556677",
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
                    DNI = "55667788",
                    BirthDate = new DateTime(1987, 9, 3),
                    PhoneNumber = "555-5005",
                    Email = "diego.rodriguez@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
        }
    }
}
