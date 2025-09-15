using CurrencyRateAggregatorService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateAggregatorService.Application.Contracts
{
    public interface IRateRepository
    {
        public Task<Rate?> GetRateByDateAsync(DateOnly date, CancellationToken ct);
        public Task<IReadOnlyList<Rate>> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
        public Task<decimal> GetAvarageAmountByRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
        public Task Upsert (Rate rate, CancellationToken ct);
    }
}
