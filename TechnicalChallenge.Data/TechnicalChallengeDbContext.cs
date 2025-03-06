using Microsoft.EntityFrameworkCore;
using TechnicalChallenge.Data.Domain;

namespace TechnicalChallenge.Data
{
    public class TechnicalChallengeDbContext : DbContext
    {
        public TechnicalChallengeDbContext(DbContextOptions<TechnicalChallengeDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } 
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<LoanRate> LoanRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountType)
                .WithMany()
                .HasForeignKey(a => a.AccountTypeId);
        }
    }
}
