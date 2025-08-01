using Microsoft.EntityFrameworkCore;
using ShopOnline.Models;

namespace ShopOnline.Context
{
    public class ShopOnlineContext : DbContext
    {
        public ShopOnlineContext(DbContextOptions<ShopOnlineContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContactU> ContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USER
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.News)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCT
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .Property(p => p.ColorId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.UserId)
                .IsRequired();

            // ORDER
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderID);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // ORDER DETAILS // Composite key
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderID, od.ProductID });

            // NEWS
            modelBuilder.Entity<News>()
                .HasKey(n => n.NewsId);

            modelBuilder.Entity<News>()
                .Property(n => n.UserId)
                .IsRequired();

            // MODEL
            modelBuilder.Entity<Model>()
                .HasKey(m => m.ModelId);

            modelBuilder.Entity<Model>()
                .HasMany(m => m.Products)
                .WithOne(p => p.Model)
                .HasForeignKey(p => p.ModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // COLOR
            modelBuilder.Entity<Color>()
                .HasKey(c => c.ColorId);

            modelBuilder.Entity<Color>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Color)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

            // CATEGORY
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // CONTACT
            modelBuilder.Entity<ContactU>()
                .HasKey(c => c.id);
        }
    }
}