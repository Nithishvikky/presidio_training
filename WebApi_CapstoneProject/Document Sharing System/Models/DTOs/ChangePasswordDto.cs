using System.ComponentModel.DataAnnotations;

namespace DSS.Models.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9@#$%&]{8,20}$", ErrorMessage = "Password must be 8-20 characters")]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9@#$%&]{8,20}$", ErrorMessage = "Password must be 8-20 characters")]
        public string NewPassword { get; set; } = string.Empty;
    }
}