using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyRateAggregatorService.Infrastructure.Service
{
    public sealed class RateSeeder : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RateSeeder> _logger;

        public RateSeeder(IServiceProvider services, ILogger<RateSeeder> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRateRepository>();
            var provider = scope.ServiceProvider.GetRequiredService<IRateProvider>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var startDate = today.AddMonths(-3);

            _logger.LogInformation("Seeding rates from {Start} to {End}", startDate, today);

            var count = 0;
            try
            {
                var providerQuotes = await provider.GetRangeOfRatesAsync(startDate, today, ct);

                if (providerQuotes is null || providerQuotes.Count == 0)
                {
                    _logger.LogInformation($"Provider returned no rates for range {startDate} – {today}");
                    return;
                }

                foreach (var quote in providerQuotes)
                {
                    if (ct.IsCancellationRequested) break;

                    var existing = await repo.GetRateByDateAsync(quote.ExchangeDate, ct);
                    if (existing != null)
                    {
                        _logger.LogInformation($"Rate for {quote.ExchangeDate} already exists, skipping.");
                        continue;
                    }

                    var rate = quote.ToRate();
                    await repo.Upsert(rate, ct);
                    _logger.LogInformation($"Rate for {quote.ExchangeDate} inserted.");

                    count++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding rates from {Start} to {End}", startDate, today);
            }

            _logger.LogInformation($"Rate seeding completed. Seeded {count} new records.");
        }
    }
}
