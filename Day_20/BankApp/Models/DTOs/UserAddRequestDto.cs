namespace Bank.Models.DTOs
{
    public class UserAddRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}