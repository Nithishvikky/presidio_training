
using System.ComponentModel.DataAnnotations;

namespace TwitterApp.Models
{
    public class Hashtag
    {
        [Key]
        public int Id { get; set; }
        public string TagName { get; set; } = string.Empty;

        public ICollection<PostHashtag>? Posts { get; set; }
    }
}