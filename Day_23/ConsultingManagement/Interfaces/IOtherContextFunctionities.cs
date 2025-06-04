using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IOtherContextFunctionities
    {
        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string specilaity);
    }
}