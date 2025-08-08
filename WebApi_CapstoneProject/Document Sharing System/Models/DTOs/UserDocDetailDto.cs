
namespace DSS.Models.DTOs
{
    public class UserDocDetailDto
    {
        public Guid DocId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public byte[]? FileData { get; set; }
        
        public bool IsDeleted { get; set; }
        public long Size { get; set; }

        public DateTime UploadedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public DateTime? TemporarilyUnarchivedAt { get; set; }
        public DateTime? ScheduledRearchiveAt { get; set; }

        public string UploaderUsername { get; set; } = string.Empty;
        public string UploaderEmail { get; set; } = string.Empty;
        public string LastViewerName { get; set; } = string.Empty;
    }
}