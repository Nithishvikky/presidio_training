using FirstAPI.Models;

namespace FirstAPI.Interfaces
{
    public interface IAppointmentService
    {
        int AddAppointment(Appointment appointment);
        Appointment GetAppointment(int id);
        List<Appointment>? AllAppointments();
    }
}
