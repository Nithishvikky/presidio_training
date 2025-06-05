namespace Notify.Models.DTOs
{
    public class UserLoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}