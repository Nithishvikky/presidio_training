
using System.ComponentModel.DataAnnotations.Schema;
using TwitterApp.Contexts;

namespace TwitterApp.Models
{
    public class PostHashtag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int HashtagId { get; set; }
    }
}