using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Models.Providers;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyRateAggregatorService.Infrastructure.Caching
{
    public class CachedRateProvider : IRateProvider
    {
        private readonly IRateProvider _inner;
        private readonly IMemoryCache _cache;

        public string Name => $"{_inner.Name} (Cached)";

        public CachedRateProvider(IRateProvider rateProvider, IMemoryCache cache)
        {
            _inner = rateProvider;
            _cache = cache;
        }

        public async Task<ProviderQuote?> GetRateByDateAsync(DateOnly date, CancellationToken ct)
        {
            string key = BuildKey(date);

            if(_cache.TryGetValue(key, out ProviderQuote? cached))
            {
                return cached;
            }

            var requestValue = await _inner.GetRateByDateAsync(date, ct);

            if(requestValue != null)
            {
                _cache.Set(key, requestValue, GetDuration());
            }

            return requestValue;
        }

        public async Task<IReadOnlyCollection<ProviderQuote>?> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct)
        {
            
            var results = await _inner.GetRangeOfRatesAsync(startDate, endDate, ct);

            if (results == null || results.Count == 0)
                return results;

            
            foreach (var quote in results)
            {
                string key = BuildKey(quote.ExchangeDate);
                _cache.Set(key, quote, GetDuration());
            }

            return results;
        }

        private string BuildKey(DateOnly date) =>
            $"{_inner.Name}:USD:{date:yyyyMMdd}";

        private TimeSpan GetDuration()
        {
            return TimeSpan.FromMinutes(10);
        }

    }
}
