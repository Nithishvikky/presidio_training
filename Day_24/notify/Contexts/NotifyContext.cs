using Microsoft.EntityFrameworkCore;
using Notify.Models;


namespace Notify.Contexts
{
    public class NotifyContext : DbContext
    {
        public NotifyContext(DbContextOptions<NotifyContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Username);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Files)
                .WithOne(f => f.UploadedBy)
                .HasConstraintName("FK_User_File")
                .HasForeignKey(f => f.UploadedByUsername)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}