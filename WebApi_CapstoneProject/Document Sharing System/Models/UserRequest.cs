using System.ComponentModel.DataAnnotations;

namespace DSS.Models
{
    public class UserRequest
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public Guid DocumentId { get; set; }
        public UserDocument Document { get; set; } = null!;
        
        public string RequestType { get; set; } = string.Empty; // Temporary, UnArchieve
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string Reason { get; set; } = string.Empty;
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        
        public DateTime? AccessGrantedAt { get; set; }
        public DateTime? AccessExpiresAt { get; set; }
        public int? AccessDurationHours { get; set; } = 24;
    }
} 