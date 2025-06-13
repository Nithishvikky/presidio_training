using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class DocumentShare
    {
        [Key]
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }
        public Guid SharedWithUserId { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}