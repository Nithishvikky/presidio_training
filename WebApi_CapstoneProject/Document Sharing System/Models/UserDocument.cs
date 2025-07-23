using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class UserDocument
    {
        [Key]
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[]? FileData { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public string Status { get; set; } = "Active"; // Active, Archived, TemporarilyUnarchived
        public DateTime? ArchivedAt { get; set; }
        public DateTime? TemporarilyUnarchivedAt { get; set; }
        public DateTime? ScheduledRearchiveAt { get; set; }

        public Guid UploadedById { get; set; }
        public User? UploadedByUser { get; set; }

    }
}