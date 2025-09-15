using CurrencyRateAggregatorService.Domain;
using Microsoft.EntityFrameworkCore;


namespace CurrencyRateAggregatorService.Infrastructure.AppDbContext
{
    public class CurrencyRateDbContext : DbContext
    {
        public CurrencyRateDbContext(DbContextOptions<CurrencyRateDbContext> options) : base(options) { }

        public DbSet<Rate> Rates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(p => p.BaseCurrency).IsRequired().HasMaxLength(3);
                entity.Property(p => p.QuoteCurrency).IsRequired().HasMaxLength(3);

                entity.Property(r => r.Amount).HasPrecision(18, 6);

                entity.Ignore(r => r.RatePer1);

                entity.HasIndex(r => new { r.Date, r.BaseCurrency, r.QuoteCurrency }).IsUnique();

            });
        }
    }
    
}
    