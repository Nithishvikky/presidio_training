using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<UserNotifications> NotificationUsers { get; set; } = new List<UserNotifications>();
    }
} 