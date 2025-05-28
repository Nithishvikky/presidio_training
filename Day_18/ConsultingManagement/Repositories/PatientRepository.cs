using ConsultingManagement.Models;
using ConsultingManagement.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Repositories
{
    public  class PatientRepository : Repository<int, Patient>
    {
        protected PatientRepository(ConsultancyContext consultancyContext) : base(consultancyContext)
        {
        }

        public override async Task<Patient> Get(int key)
        {
            var patient = await _consultancyContext.patients.SingleOrDefaultAsync(p => p.Id == key);

            return patient??throw new Exception($"No patient with the given ID : {key}");
        }

        public override async Task<IEnumerable<Patient>> GetAll()
        {
            var patients = _consultancyContext.patients;
            if (patients.Count() == 0)
                throw new Exception("No Patients in the database");
            return (await patients.ToListAsync());
        }
    }
}