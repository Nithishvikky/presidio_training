
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;

namespace ConsultingManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        IRepository<int, Appointment> _appointmentRepository;

        public AppointmentService(IRepository<int,Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public string? AddAppointment(Appointment appointment)
        {
             try
            {
                var res = _appointmentRepository.Add(appointment);
                if(res != null)
                {
                    return res.AppointmnetNumber;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;
        }

        public List<Appointment>? AllAppointments()
        {
            try
            {
                return _appointmentRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public Appointment? GetAppointment(int id)
        {
            try
            {
                var appointment = _appointmentRepository.GetById(id);
                if (appointment != null)
                {
                    return appointment;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}