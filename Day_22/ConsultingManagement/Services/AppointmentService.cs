using ConsultingManagement.Interfaces;
using ConsultingManagement.Misc;
using ConsultingManagement.Models;
using ConsultingManagement.Models.DTOs;


namespace ConsultingManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Patient> _patientRepository;
        private readonly IRepository<string, Appointment> _appointmentRepository;

        public AppointmentService(IRepository<int, Doctor> doctorRepository,
                                IRepository<int, Patient> patientRepository,
                                IRepository<string, Appointment> appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment)
        {
            try
            {
                var newAppointment = new AppointmentMapper().MapAppointment(appointment);
                newAppointment.AppointmentNumber = GetAppointmentNumber(newAppointment.AppointmentDateTime, newAppointment.PatientId);

                var patient = await _patientRepository.Get(newAppointment.PatientId);

                if (patient == null)
                {
                    throw new Exception("No patient found with ID");
                }
                newAppointment.Patient = patient;

                var doctor = await _doctorRepository.Get(newAppointment.DoctorId);

                if (doctor == null)
                {
                    throw new Exception("No doctor found with ID");
                }
                newAppointment.Doctor = doctor;

                newAppointment = await _appointmentRepository.Add(newAppointment);

                return newAppointment;

            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong..{e.Message}");
            }
        }
        public string GetAppointmentNumber(DateTime dateTime, int PatientId)
        {
            return "A" + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2")
                    + "P" + PatientId.ToString("D4");
        }
        public async Task<Appointment> CancelAppointment(string appointmentNumber)
        {
            try
            {
                var appointment = await _appointmentRepository.Get(appointmentNumber);
                if (appointment == null)
                {
                    throw new Exception("No appointment found");
                }

                appointment.Status = "Cancelled";
                appointment = await _appointmentRepository.Update(appointmentNumber, appointment);
                return appointment;
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong...{e.Message}");
            }
        }
    }
}