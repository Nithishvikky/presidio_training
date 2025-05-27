using Microsoft.EntityFrameworkCore;
using TwitterApp.Models;


namespace TwitterApp.Contexts
{
    public class TwitterContext : DbContext
    {
        public TwitterContext(DbContextOptions options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Hashtag> hashtags { get; set; }
        public DbSet<PostHashtag> postHashtags { get; set; }
        public DbSet<Like> likes { get; set; }
        public DbSet<Following> followings{ get; set; }
    }
}