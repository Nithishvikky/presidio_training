namespace DSS.Models.DTOs
{
    public class UserDocDetailDto
    {
        public Guid DocId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}