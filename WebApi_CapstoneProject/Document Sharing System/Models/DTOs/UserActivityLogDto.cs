namespace DSS.Models.DTOs
{
    public class UserActivityLogDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserUsername { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class CreateActivityLogDto
    {
        public Guid UserId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UserActivitySummaryDto
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserUsername { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public DateTime? LastActivity { get; set; }
        public int TotalActivities { get; set; }
        public int LoginCount { get; set; }
        public int DocumentUploads { get; set; }
        public int DocumentShares { get; set; }
    }
} 