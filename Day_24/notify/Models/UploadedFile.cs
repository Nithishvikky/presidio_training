
namespace Notify.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[]? FileData { get; set; }

        public string UploadedByUsername { get; set; } = string.Empty;
        public User? UploadedBy { get; set; }
    }
}