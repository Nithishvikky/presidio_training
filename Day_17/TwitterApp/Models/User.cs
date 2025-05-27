
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwitterApp.Contexts;

namespace TwitterApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public ICollection<Post>? Posts { get; set; }  
        public ICollection<Following>?  Followings { get; set; } 
    }
}