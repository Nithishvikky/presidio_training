using Microsoft.EntityFrameworkCore;
using VMTask.Models;

namespace VMTask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<Person> People { get; set; }
    }
}
