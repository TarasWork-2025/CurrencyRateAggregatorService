using CurrencyRateAggregatorService.Application.Models.Providers;

namespace CurrencyRateAggregatorService.Application.Contracts
{
    public interface IRateProvider
    {
        public string Name { get; }
        public Task<ProviderQuote?> GetRateByDateAsync(DateOnly date, CancellationToken ct);
        public Task<IReadOnlyCollection<ProviderQuote>?> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
    }
}
