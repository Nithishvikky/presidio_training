using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Contexts
{
    public class DssContext : DbContext
    {
        public DssContext(DbContextOptions<DssContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<AuthSession>AuthSessions{ get; set; }

        public DbSet<DocumentView> DocumentViews{ get; set; }
        public DbSet<DocumentShare> DocumentShares{ get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.UploadedDocuments)
                .WithOne(f => f.UploadedByUser)
                .HasConstraintName("FK_User_File")
                .HasForeignKey(f => f.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuthSession>()
                .HasOne(e => e.User)
                .WithMany(u => u.Sessions)
                .HasConstraintName("FK_User_Auth")
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DocumentView>(entity =>
            {
                entity.HasOne(e => e.Document)
                    .WithMany()
                    .HasConstraintName("FK_View_Document")
                    .HasForeignKey(e => e.DocumentId)
                    .OnDelete(DeleteBehavior.Restrict); ;

                entity.HasOne(e => e.ViewedBy)
                    .WithMany()
                    .HasConstraintName("FK_View_User")
                    .HasForeignKey(e => e.ViewedByUserId)
                    .OnDelete(DeleteBehavior.Restrict); ;
            });

            modelBuilder.Entity<DocumentShare>()
                    .HasOne<UserDocument>()
                    .WithMany()
                    .HasConstraintName("FK_Shared_User")
                    .HasForeignKey(ds => ds.DocumentId);

            modelBuilder.Entity<DocumentShare>()
                    .HasOne<User>()
                    .WithMany()
                    .HasConstraintName("FK_Shared_Document")
                    .HasForeignKey(ds => ds.SharedWithUserId);

            modelBuilder.Entity<UserNotifications>(entity =>
            {
                entity.HasOne(e => e.Notification)
                    .WithMany(n => n.NotificationUsers)
                    .HasConstraintName("FK_UserNotification_Notification")
                    .HasForeignKey(e => e.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserNotifications)
                    .HasConstraintName("FK_UserNotification_User")
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ActivityLogs)
                    .HasConstraintName("FK_UserActivityLog_User")
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRequest>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Requests)
                    .HasConstraintName("FK_UserRequest_User")
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Document)
                    .WithMany()
                    .HasConstraintName("FK_UserRequest_Document")
                    .HasForeignKey(e => e.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                
            });
        }
    }
}