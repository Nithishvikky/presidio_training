
using System.ComponentModel.DataAnnotations.Schema;

namespace TwitterApp.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }

        [ForeignKey("UserId")]
        public User? LikedUser { get; set; }
    }
}