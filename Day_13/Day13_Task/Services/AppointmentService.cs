using Day13_Task.Interfaces;
using Day13_Task.Models;

namespace Day13_Task.Services
{
    public class AppointmentService : IAppointmentService
    {
        IRepositor<int, Appointment> _appointmentRepository;
        public AppointmentService(IRepositor<int, Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var res = _appointmentRepository.Add(appointment);
                if(res != null)
                {
                    return res.Id;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel)
        {
            try
            {
                var appointments = _appointmentRepository.GetAll();
                appointments = SearchByName(appointments, searchModel.PatientName);
                appointments = SearchByAge(appointments, searchModel.AgeRange);
                appointments = SearchByDate(appointments, searchModel.AppointmentDate);
                if (appointments != null && appointments.Count > 0)
                    return appointments.ToList(); ;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private ICollection<Appointment>? SearchByName(ICollection<Appointment> appointments,string? name)
        {
            if(string.IsNullOrEmpty(name) || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            return appointments.Where(a => a.PatientName.ToLower().Contains(name.ToLower())).ToList();
        }
        private ICollection<Appointment>? SearchByDate(ICollection<Appointment> appointments,DateTime? aDate)
        {
            if (aDate == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            return appointments.Where(a => a.AppointmentDate.Equals(aDate)).ToList();
        }

        private ICollection<Appointment>? SearchByAge(ICollection<Appointment> appointments, Range<int>? age)
        {
            if (age == null || appointments.Count == 0 || appointments == null)
            {
                return appointments;
            }
            // Will return sorted list by age
            return appointments.Where(a => a.PatientAge >= age.MinVal && a.PatientAge <= age.MaxVal).ToList();
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
    }
}
