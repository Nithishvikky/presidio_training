namespace ConsultingManagement.Models.DTOs
{
    public class DoctorAddRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<SpecialityAddRequestDto>? Specialities { get; set; }
        public float YearsOfExperience { get; set; }
        public string Email { get; set; }= string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}