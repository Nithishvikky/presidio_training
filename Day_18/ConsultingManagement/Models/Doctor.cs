namespace ConsultingManagement.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public float YearsOfExperience { get; set; }
        public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
        public ICollection<Appointment>? Appointmnets { get; set; }
    }
}