using CurrencyRateAggregatorService.Application.Common;
using CurrencyRateAggregatorService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateAggregatorService.Application.Contracts
{
    public interface IRateService
    {
        Task<ValidationResult<Rate?>> GetRateByDateAsync(DateOnly date, CancellationToken ct);
        Task<ValidationResult<decimal?>> GetAverageRateAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct);
    }
}
