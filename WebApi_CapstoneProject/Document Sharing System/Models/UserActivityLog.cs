using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class UserActivityLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public string ActivityType { get; set; } = string.Empty; // Login, Logout, DocumentUpload, DocumentShare, etc.
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 