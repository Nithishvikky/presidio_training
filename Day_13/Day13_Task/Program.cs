using System.Runtime.CompilerServices;
using Day13_Task.Interfaces;
using Day13_Task.Models;
using Day13_Task.Repositories;
using Day13_Task.Services;

namespace Day13_Task
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IRepositor<int, Appointment> appointmentRepository = new AppointmentRepository();
            IAppointmentService appointmentService = new AppointmentService(appointmentRepository);
            AppointmentUI appointmentUI = new AppointmentUI(appointmentService);
            appointmentUI.Run();
        }
    }
}
