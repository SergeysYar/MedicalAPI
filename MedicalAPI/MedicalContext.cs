using MedicalAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace MedicalAPI
{
    public class MedicalContext : DbContext
    {
        public MedicalContext(DbContextOptions<MedicalContext> options) : base(options) { }

        public DbSet<Section> Sections { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связей и ограничений
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Section)
                .WithMany()
                .HasForeignKey(p => p.SectionId);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Room)
                .WithMany()
                .HasForeignKey(d => d.RoomId);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialization)
                .WithMany()
                .HasForeignKey(d => d.SpecializationId);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Section)
                .WithMany()
                .HasForeignKey(d => d.SectionId);
        }
    }
}
