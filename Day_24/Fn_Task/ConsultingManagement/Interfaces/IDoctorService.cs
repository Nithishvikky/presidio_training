using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IDoctorService
    {
        public Task<Doctor?> GetDoctByEmail(string email);
        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality);
        public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
    }
}