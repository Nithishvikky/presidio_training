using ConsultingManagement.Models;

namespace ConsultingManagement.Interfaces
{
    public interface IAppointmentService
    {
        string? AddAppointment(Appointment appointment);
        Appointment? GetAppointment(int id);
        List<Appointment>? AllAppointments();
    }
}
