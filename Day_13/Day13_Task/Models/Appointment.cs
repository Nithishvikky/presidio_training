using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Day13_Task.Models
{
    public class Appointment : IComparable<Appointment>
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;

        public Appointment() { }

        public Appointment(int id, string patientName, int patientAge, DateTime appointmentDate, string reason)
        {
            Id = id;
            PatientName = patientName;
            PatientAge = patientAge;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }

        public void GetPatientDetailsFromUSer()
        {
            Console.Write("Please enter the Patient name : ");
            PatientName = Console.ReadLine() ?? "";

            Console.Write("Please enter the Patient age : ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
            {
                Console.WriteLine("Invalid entry for age...");
                Console.Write("Please enter the Patient age : ");
            }
            PatientAge = age;

            Console.Write("Please enter the appointment date(yyyy-MM-dd) : ");
            DateTime appoinmentDate;
            while (!DateTime.TryParse(Console.ReadLine(), out appoinmentDate) || appoinmentDate < DateTime.Today)
            {
                Console.WriteLine("Invalid entry for date...");
                Console.Write("Please enter the appointment date(yyyy-MM-dd) : ");
            }
            AppointmentDate = appoinmentDate;

            Console.Write("Please enter the reason for appointment : ");
            Reason = Console.ReadLine() ?? "";
        }

        public override string ToString()
        {
            return "Appointment ID : " + Id + "\nName : " + PatientName + "\nAge : " + PatientAge + "\nDate : " + AppointmentDate;
        }

        // Age comparison
        public int CompareTo(Appointment? other)
        {
            return this.PatientAge.CompareTo(other?.PatientAge);
        }
    }

}
