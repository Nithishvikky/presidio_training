using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.MinValue;
        public DateTime? LastLogin { get; set; }


        public ICollection<UserDocument>? UploadedDocuments { get; set; }
        public ICollection<AuthSession>? Sessions { get; set; }
        public ICollection<UserNotifications>? UserNotifications { get; set; }
        public ICollection<UserActivityLog>? ActivityLogs { get; set; }
        public ICollection<UserRequest>? Requests { get; set; }

    }
}