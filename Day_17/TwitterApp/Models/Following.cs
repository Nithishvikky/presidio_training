
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Following
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FollowerId { get; set; }

        [ForeignKey("FollowerId")]
        public User? User { get; set; }
    }
}