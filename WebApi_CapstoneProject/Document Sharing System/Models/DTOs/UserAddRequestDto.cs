using System.ComponentModel.DataAnnotations;
using DSS.Misc;

namespace DSS.Models.DTOs
{
    public class UserAddRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9@#$%&]{8,20}$", ErrorMessage = "Password must be 8-20 characters")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [RoleValidation]
        public string Role { get; set; } = string.Empty;
    }
}