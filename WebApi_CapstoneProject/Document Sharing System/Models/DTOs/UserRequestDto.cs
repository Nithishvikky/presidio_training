using System.ComponentModel.DataAnnotations;

namespace DSS.Models.DTOs
{
    public class UserRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserUsername { get; set; } = string.Empty;
        public Guid DocumentId { get; set; }
        public string DocumentFileName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? AccessGrantedAt { get; set; }
        public DateTime? AccessExpiresAt { get; set; }
        public int? AccessDurationHours { get; set; }
    }

    public class CreateUserRequestDto
    {
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RequestType { get; set; } = "Temporary";
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [Range(1, 168)] // 1 hour to 1 week
        public int? AccessDurationHours { get; set; } = 24;
    }

    public class ProcessUserRequestDto
    {
        public Guid RequestId { get; set; }
        public string Status { get; set; } = string.Empty; // Approved, Rejected
        public int? AccessDurationHours { get; set; } = 24;
    }

    public class DocumentAccessStatusDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentFileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? AccessExpiresAt { get; set; }
        public bool HasAccess { get; set; }
        public bool IsExpired { get; set; }
    }
} 