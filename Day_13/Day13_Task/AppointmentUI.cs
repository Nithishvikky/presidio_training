using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day13_Task.Services;
using Day13_Task.Interfaces;
using Day13_Task.Models;
using System.Runtime.InteropServices;

namespace Day13_Task
{
    public class AppointmentUI
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentUI(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public void Run()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Choose from options...\n\t1. Add appointment\n\t2. Search Appointments\n\t3. List All appointments");
                int op = UserInputNumValidation();
                switch (op)
                {
                    case 1:
                        BookAppointment();
                        break;
                    case 2:
                        searchAppointments();
                        break;
                    case 3:
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Enter valid Option....");
                        break;
                }
            }
        }
        static string UserInputValidation()
        {
            string? input;
            while (String.IsNullOrEmpty(input = Console.ReadLine()?.Trim()))
            {
                Console.Write("Enter valid word : ");
            }

            return input;
        }
        static int UserInputNumValidation()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Enter valid number : ");
            }
            return num;
        }
        public void BookAppointment()
        {
            Appointment a = new Appointment();
            a.GetPatientDetailsFromUSer();
            int id = _appointmentService.AddAppointment(a);
            Console.WriteLine($"Appointment Booked.\nThe AppointmentId is {id}");
        }
        public void searchAppointments()
        {
            var appointments = _appointmentService.SearchAppointment(SearchCategories());
            if (appointments == null)
            {
                Console.WriteLine("No appointments with this details");
            }
            else
            {
                PrintAppointments(appointments);
            }
        }
        public AppointmentSearchModel SearchCategories()
        {
            AppointmentSearchModel searchModel = new AppointmentSearchModel();
            searchModel.GetSearchDetailsFromUSer();
            return searchModel;
        }

        private void PrintAppointments(List<Appointment>? appointments)
        {
            foreach (var item in appointments)
            {
                Console.WriteLine(item);
            }
        }
    }
}
