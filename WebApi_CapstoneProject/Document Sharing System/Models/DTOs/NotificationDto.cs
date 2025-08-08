namespace DSS.Models.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }

    public class MarkNotificationReadDto
    {
        public Guid NotificationId { get; set; }
    }
} 