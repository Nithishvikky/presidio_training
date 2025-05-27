using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingManagement.Models
{
    public class Appointment
    {
        [Key]
        public string AppointmnetNumber { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmnetDateTime { get; set; }

        public string Status { get; set; } = string.Empty;

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
    }
}