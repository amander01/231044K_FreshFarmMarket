using _231044K_FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _231044K_FreshFarmMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // ✅ Password history table (you already added earlier)
        public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AuditLog>()
                .HasIndex(x => x.UserId);

            builder.Entity<PasswordHistory>()
                .HasIndex(x => x.UserId);

            builder.Entity<PasswordHistory>()
                .HasIndex(x => x.CreatedAt);
        }
    }
}
