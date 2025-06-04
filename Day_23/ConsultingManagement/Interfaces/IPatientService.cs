using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IPatientService
    {
        public Task<Patient> AddPatient(PatientAddRequestDto patient);
    }
}