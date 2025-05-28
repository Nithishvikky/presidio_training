using ConsultingManagement.Models;
using ConsultingManagement.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Repositories
{
    public  class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
    {
        protected DoctorSpecialityRepository(ConsultancyContext consultancyContext) : base(consultancyContext)
        {
        }

        public override async Task<DoctorSpeciality> Get(int key)
        {
            var doctorSpeciality = await _consultancyContext.doctorSpecialities.SingleOrDefaultAsync(dsp => dsp.SerialNumber == key);

            return doctorSpeciality??throw new Exception($"No doctorSpeciality with the given ID : {key}");
        }

        public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
        {
            var doctorSpecialities = _consultancyContext.doctorSpecialities;
            if (doctorSpecialities.Count() == 0)
                throw new Exception("No specialities in the database");
            return (await doctorSpecialities.ToListAsync());
        }
    }
}