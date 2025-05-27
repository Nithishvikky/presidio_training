
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingManagement.Models
{
    public class DoctorSpeciality
    {
        [Key]
        public int SerialNumber { get; set; }
        public int DoctorId { get; set; }
        public int SpecialityId { get; set; }

        [ForeignKey("SpecialityId")]
        public Speciality? Speciality { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }   
    }
}