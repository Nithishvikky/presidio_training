using System.ComponentModel.DataAnnotations;

namespace ConsultingManagement.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public byte[]? Password { get; set; }

        public byte[]? HashKey { get; set; }
        public Patient? Patient { get; set; }
        public Doctor? Doctor{ get; set; }
    }
}