namespace DSS.Models.DTOs
{
    public class SharedResponseeDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime GrantedAt{ get; set; }
    }
}