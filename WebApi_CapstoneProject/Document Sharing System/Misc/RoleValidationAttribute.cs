using System.ComponentModel.DataAnnotations;

namespace DSS.Misc
{
    public class RoleValidationAttribute : ValidationAttribute
    {
        private readonly string[] _allowedRoles = new[] { "Admin", "User" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string role && _allowedRoles.Contains(role))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Role must be one of the following: {string.Join(", ", _allowedRoles)}");
        }
    }
}