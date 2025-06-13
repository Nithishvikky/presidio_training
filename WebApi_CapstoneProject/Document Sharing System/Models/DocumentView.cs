using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class DocumentView
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid ViewedByUserId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        public UserDocument? Document { get; set; }
        public User? ViewedBy { get; set; }
    }
}
