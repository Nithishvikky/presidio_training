using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;


namespace ConsultingManagement.Misc
{
    public class PatientMapper
    {
        public Patient? MapPatientAddRequest(PatientAddRequestDto addRequestDto)
        {
            Patient patient = new();
            patient.Name = addRequestDto.Name;
            patient.Age = addRequestDto.Age;
            patient.Email = addRequestDto.Email;
            return patient;
        }
    }
}