using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Misc
{
    public class AppointmentMapper
    {
        public Appointment MapAppointment(AppointmentAddRequestDto addRequestDto)
        {
            Appointment appointment = new();
            appointment.PatientId = addRequestDto.PatientId;
            appointment.DoctorId = addRequestDto.DoctorId;
            appointment.AppointmentDateTime = addRequestDto.AppointmentDate;
            appointment.Status = "Scheduled";
            return appointment;
        }
    }
}