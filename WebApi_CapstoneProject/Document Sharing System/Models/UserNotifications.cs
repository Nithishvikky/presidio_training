using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class UserNotifications
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
} 