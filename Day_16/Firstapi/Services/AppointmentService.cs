
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Repositories;

namespace FirstAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        IRepository<int, Appointment> _appointmentRepostiory;

        public AppointmentService(IRepository<int,Appointment> appointmentRepository)
        {
            _appointmentRepostiory = appointmentRepository;
        }
        public int AddAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public List<Appointment>? AllAppointments()
        {
            throw new NotImplementedException();
        }

        public Appointment GetAppointment(int id)
        {
            throw new NotImplementedException();
        }
    }
}