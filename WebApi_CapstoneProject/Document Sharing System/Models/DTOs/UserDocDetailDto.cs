
namespace DSS.Models.DTOs
{
    public class UserDocDetailDto
    {
        public Guid DocId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
        public byte[]? FileData { get; set; }

        public string LastViewerName { get; set; } = string.Empty;
        public string UploaderUsername { get; set; } = string.Empty;
        public string UploaderEmail { get; set; } = string.Empty;
    }
}