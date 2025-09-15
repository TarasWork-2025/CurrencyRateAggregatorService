using CurrencyRateAggregatorService.Domain;
using CurrencyRateAggregatorService.Infrastructure.AppDbContext;
using CurrencyRateAggregatorService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;


namespace CurrencyRateAggregatorService.Tests.Infrastructure
{
    public class RateRepositoryTests
    {
        private CurrencyRateDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<CurrencyRateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new CurrencyRateDbContext(options);
        }

        [Fact]
        public async Task Upsert_Should_Insert_New_Rate()
        {
            using var db = GetDbContext();
            var repo = new RateRepository(db, NullLogger<RateRepository>.Instance);

            var rate = new Rate(Guid.NewGuid(),DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);

            await repo.Upsert(rate, default);

            Assert.Single(db.Rates);
        }

        [Fact]
        public async Task Upsert_Should_Update_Existing_Rate()
        {
            using var db = GetDbContext();
            var repo = new RateRepository(db, NullLogger<RateRepository>.Instance);

            var id = Guid.NewGuid();
            var rate = new Rate(id, DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);
            db.Rates.Add(rate);
            await db.SaveChangesAsync();

            var updated = new Rate(id, rate.Date, "USD", "UAH", 1, 40m);
            await repo.Upsert(updated, default);

            var saved = db.Rates.First();
            Assert.Equal(40m, saved.Amount);
        }

        [Fact]
        public async Task GetRateByDateAsync_Should_Return_Rate_When_Found()
        {
            using var db = GetDbContext();
            var repo = new RateRepository(db, NullLogger<RateRepository>.Instance);

            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var rate = new Rate(Guid.NewGuid(), date, "USD", "UAH", 1, 36.6m);
            db.Rates.Add(rate);
            await db.SaveChangesAsync();

            var result = await repo.GetRateByDateAsync(date, default);

            Assert.NotNull(result);
            Assert.Equal(36.6m, result.Amount);
        }

        [Fact]
        public async Task GetAverageRateAsync_Should_Return_Average()
        {
            using var db = GetDbContext();
            var repo = new RateRepository(db, NullLogger<RateRepository>.Instance);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            db.Rates.Add(new Rate(Guid.NewGuid(), today.AddDays(-1), "USD", "UAH", 1, 36m));
            db.Rates.Add(new Rate(Guid.NewGuid(),today, "USD", "UAH", 1, 38m));
            await db.SaveChangesAsync();

            var avg = await repo.GetAvarageAmountByRangeAsync(today.AddDays(-1), today, default);

            Assert.Equal(37m, avg);
        }
    }
}
