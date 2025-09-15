using CurrencyRateAggregatorService.Application.Common;
using CurrencyRateAggregatorService.Domain;

namespace CurrencyRateAggregatorService.Application.Contracts
{
    public interface IRateService
    {
        Task<ValidationResult<Rate?>> GetRateByDateAsync(DateOnly date, CancellationToken ct);
        Task<ValidationResult<decimal?>> GetAverageRateAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
    }
}
