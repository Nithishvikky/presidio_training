using System.ComponentModel.DataAnnotations;

namespace VT.Models
{
    public class UploadVideoRequestDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}