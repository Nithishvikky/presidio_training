
namespace ConsultingManagement.Models
{
    public class Speciality
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
    }
}