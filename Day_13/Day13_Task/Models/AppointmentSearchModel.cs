using System;
using static System.Net.Mime.MediaTypeNames;

namespace Day13_Task.Models
{
	public class AppointmentSearchModel()
	{
        public string? PatientName { get; set; } 
        public DateTime? AppointmentDate { get; set; }
        public Range<int>? AgeRange { get; set; }

        public void GetSearchDetailsFromUSer()
        {
            Console.Write("Name of the Patients to filter : ");
            PatientName = Console.ReadLine() ?? "";

            Console.Write("Min Age of the Patients to filter : ");
            string? minAgeStr = Console.ReadLine();
            Console.Write("Max Age of the Patients to filter : ");
            string? maxAgeStr = Console.ReadLine();
            if (int.TryParse(minAgeStr, out int minAge) && int.TryParse(maxAgeStr, out int maxAge))
                AgeRange = new Range<int> { MinVal = minAge, MaxVal = maxAge };

            Console.Write("Appointment date of the Patients to filter : ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                AppointmentDate = date;
            }
        }

    }

    public class Range<T>
    {
        public T? MinVal { get; set; }
        public T? MaxVal { get; set; }
    }
}
