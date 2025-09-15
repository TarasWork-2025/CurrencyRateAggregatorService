using CurrencyRateAggregatorService.Application.Models.Providers;
using CurrencyRateAggregatorService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateAggregatorService.Application.Contracts
{
    public interface IRateProvider
    {
        public string Name { get; }
        public Task<ProviderQuote?> GetRateByDateAsync(DateOnly date, CancellationToken ct);
        public Task<IReadOnlyCollection<ProviderQuote>?> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
    }
}
