using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Domain;
using CurrencyRateAggregatorService.Infrastructure.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CurrencyRateAggregatorService.Infrastructure.Repositories
{
    public class RateRepository : IRateRepository
    {
        private readonly CurrencyRateDbContext _dbContext;
        private readonly ILogger<RateRepository> _logger;

        public RateRepository(CurrencyRateDbContext dbContext, ILogger<RateRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Rate?> GetRateByDateAsync(DateOnly date, CancellationToken ct)
        {
            try
            {
                return await _dbContext.Rates
                    .FirstOrDefaultAsync(r => r.Date == date, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rate for {Date}", date);
                throw;
            }
        }

        public async Task<IReadOnlyList<Rate>> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct)
        {
            try
            {
                var rangeOfRates = await _dbContext.Rates.Where(r => r.Date >= startDate && r.Date <= endDate)
                                                                     .AsNoTracking()
                                                                     .ToListAsync(ct);

                return rangeOfRates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rates for {Start} and {End}", startDate, endDate);
                throw;
            }
            
        }

        public async Task<decimal> GetAvarageAmountByRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct)
        {
            try
            {
                return await _dbContext.Rates
                    .Where(r => r.Date >= startDate && r.Date <= endDate)
                    .Select(r => r.Amount / r.Units)
                    .DefaultIfEmpty()
                    .AverageAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average rate between {Start} and {End}", startDate, endDate);
                throw;
            }
        }

        public async Task Upsert(Rate rate, CancellationToken ct)
        {
            try
            {
                var existingRate = await _dbContext.Rates
                    .FirstOrDefaultAsync(r => r.Id == rate.Id, ct);

                if (existingRate != null)
                {
                    existingRate.UpdateBaseCurrency(rate.BaseCurrency);
                    existingRate.UpdateQuoteCurrency(rate.QuoteCurrency);
                    existingRate.UpdateAmount(rate.Amount);
                    existingRate.UpdateUnits(rate.Units);

                    _logger.LogInformation("Updated rate {Id} for {Date}", rate.Id, rate.Date);
                }
                else
                {
                    await _dbContext.Rates.AddAsync(rate, ct);
                    _logger.LogInformation("Inserted new rate {Id} for {Date}", rate.Id, rate.Date);
                }

                await _dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update failed for rate {Id} ({Date})", rate.Id, rate.Date);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Upsert for rate {Id} ({Date})", rate.Id, rate.Date);
                throw;
            }
        }
    }
}
