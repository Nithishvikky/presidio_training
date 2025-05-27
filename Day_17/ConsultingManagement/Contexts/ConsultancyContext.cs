using Microsoft.EntityFrameworkCore;
using ConsultingManagement.Models;

namespace ConsultingManagement.Contexts
{
    public class ConsultancyContext : DbContext
    {
        public ConsultancyContext(DbContextOptions options) : base(options){}
        public DbSet<Patient> patients { get; set; }
        public DbSet<Doctor> doctors { get; set; }
        public DbSet<Appointment> appointments { get; set; }
        public DbSet<Speciality> specialities { get; set; }
        public DbSet<DoctorSpeciality> doctorSpecialities { get; set; }
    }
}