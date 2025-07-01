using ConsultingManagement.Models.DTOs;
using ConsultingManagement.Contexts;
using ConsultingManagement.Interfaces;


namespace ConsultingManagement.Misc
{
    public class OtherFuncinalitiesImplementation : IOtherContextFunctionities
    {
        private readonly ConsultancyContext _consultancyContext;

        public OtherFuncinalitiesImplementation(ConsultancyContext consultancyContext)
        {
            _consultancyContext = consultancyContext;
        }

        public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string specilaity)
        {
            var result = await _consultancyContext.GetDoctorsBySpeciality(specilaity);
            return result;
        }
    }
}