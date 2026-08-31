using consultorio_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace consultorio_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure TPT (Table Per Type) inheritance mapping
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<Psychologist>().ToTable("Psychologists");
        }
    }
}
