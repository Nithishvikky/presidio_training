using System.ComponentModel.DataAnnotations;

namespace Notify.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public string Role { get; set; } = string.Empty;
        public byte[]? HashKey { get; set; }
        
        public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
    }
}