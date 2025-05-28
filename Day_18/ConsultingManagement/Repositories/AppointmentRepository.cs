using Microsoft.EntityFrameworkCore;
using ConsultingManagement.Contexts;
using ConsultingManagement.Models;

namespace ConsultingManagement.Repositories
{
    public class AppointmentRepository : Repository<string, Appointment>
    {
        protected AppointmentRepository(ConsultancyContext consultancyContext) : base(consultancyContext) { }

        public override async Task<Appointment> Get(string key)
        {
            var appointment = await _consultancyContext.appointments.SingleOrDefaultAsync(a => a.AppointmentNumber == key);
            return appointment ?? throw new Exception($"No appointment found with {key}");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = _consultancyContext.appointments;
            if (appointments.Count() == 0)
                throw new Exception("No Patients in the database");
            return (await appointments.ToListAsync());
        }
    }
}