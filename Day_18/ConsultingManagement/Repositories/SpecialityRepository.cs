using ConsultingManagement.Models;
using ConsultingManagement.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConsultingManagement.Repositories
{
    public  class SpecialityRepository : Repository<int, Speciality>
    {
        protected SpecialityRepository(ConsultancyContext consultancyContext) : base(consultancyContext)
        {
        }

        public override async Task<Speciality> Get(int key)
        {
            var speciality = await _consultancyContext.specialities.SingleOrDefaultAsync(sp => sp.Id == key);

            return speciality??throw new Exception($"No speciality with the given ID : {key}");
        }

        public override async Task<IEnumerable<Speciality>> GetAll()
        {
            var specialities = _consultancyContext.specialities;
            if (specialities.Count() == 0)
                throw new Exception("No specialities in the database");
            return (await specialities.ToListAsync());
        }
    }
}