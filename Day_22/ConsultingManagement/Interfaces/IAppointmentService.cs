using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment);

        public Task<Appointment> CancelAppointment(string appointmentNumber);
    }
}