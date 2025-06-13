using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class AuthSession
    {
        [Key]
        public Guid Id { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public Guid UserId { get; set; }
        public User? User{ get; set; }
    }
}