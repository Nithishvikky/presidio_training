namespace ConsultingManagement.Models.DTOs
{
    public class DoctorsBySpecialityResponseDto
    {
        public int Id { get; set; }
        public string Dname { get; set; } = string.Empty;
        public float Yoe { get; set; }
    }
}