using Bank.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank.Contexts
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            modelBuilder.Entity<Account>().HasOne(a => a.User)
                                          .WithMany(a => a.Accounts)
                                          .HasForeignKey(a => a.UserId)
                                          .HasConstraintName("FK_Account_User")
                                          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>().HasKey(T => T.Id);
            modelBuilder.Entity<Transaction>().HasOne(T => T.Account)
                                              .WithMany(T => T.Transactions)
                                              .HasForeignKey(T => T.AccountId)
                                              .HasConstraintName("FK_Transaction_Account")
                                              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}