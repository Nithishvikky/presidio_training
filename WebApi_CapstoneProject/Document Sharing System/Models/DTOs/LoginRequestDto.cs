using System.ComponentModel.DataAnnotations;

namespace DSS.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9@#$%&]{8,20}$", ErrorMessage = "Password must be 8-20 characters")]
        public string Password { get; set; } = string.Empty;
    }
}