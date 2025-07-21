using Microsoft.EntityFrameworkCore;
using VT.Models;

namespace VT.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TrainingVideo> TrainingVideos { get; set; }
    }
}
