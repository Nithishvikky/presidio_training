
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitterApp.Models;

namespace TwitterApp.Contexts
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Captions { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? PostedUser { get; set; }

        public ICollection<PostHashtag>? Hashtags { get; set; }
        public ICollection<Like>? Likes{ get; set; }
    }
}