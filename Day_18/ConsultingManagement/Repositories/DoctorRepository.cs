using Microsoft.EntityFrameworkCore;
using ConsultingManagement.Models;
using ConsultingManagement.Contexts;

namespace ConsultingManagement.Repositories
{
    public class DoctorRepository : Repository<int, Doctor>
    {
        protected DoctorRepository(ConsultancyContext consultancyContext) : base(consultancyContext)
        {
        }

        public override async Task<Doctor> Get(int key)
        {
            var doctor = await _consultancyContext.doctors.SingleOrDefaultAsync(d => d.Id == key);

            return doctor??throw new Exception($"No doctor with the given ID : {key}");
        }

        public override async Task<IEnumerable<Doctor>> GetAll()
        {
            var doctors = _consultancyContext.doctors;
            if (doctors.Count() == 0)
                throw new Exception("No Doctors in the database");
            return (await doctors.ToListAsync());
        }
    }
}