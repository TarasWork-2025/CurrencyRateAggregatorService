using CurrencyRateAggregatorService.Application.Common;
using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Mappings;
using CurrencyRateAggregatorService.Domain;
using Microsoft.Extensions.Logging;


namespace CurrencyRateAggregatorService.Application.Services
{
    public class RateService : IRateService
    {
        private readonly IRateProvider _rateProvider;
        private readonly IRateRepository _rateRepository;
        private readonly ILogger<RateService> _logger;

        public RateService(IRateProvider rateProvider, IRateRepository rateRepository, ILogger<RateService> logger)
        {
            _rateProvider = rateProvider;
            _rateRepository = rateRepository;
            _logger = logger;
        }

        public async Task<ValidationResult<Rate?>> GetRateByDateAsync(DateOnly date, CancellationToken ct)
        {

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (date > today)
                return ValidationResult<Rate?>.Fail("Date cannot be in the future.");

            var existingRecord = await _rateRepository.GetRateByDateAsync(date, ct);
            if (existingRecord != null)
                return ValidationResult<Rate?>.Success(existingRecord);

            var requestRate = await _rateProvider.GetRateByDateAsync(date, ct);
            if (requestRate != null)
            {
                var rate = requestRate.ToRate();
                await _rateRepository.Upsert(rate!, ct);
                return ValidationResult<Rate?>.Success(rate);
            }

            return ValidationResult<Rate?>.Success(null);
        }

        public async Task<ValidationResult<decimal?>> GetAverageRateAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var earliestAllowed = today.AddMonths(-3);

            if (startDate < earliestAllowed || endDate < earliestAllowed)
            {
                _logger.LogWarning($"Requested average for range {startDate} - {endDate} older than 3 months");
                return ValidationResult<decimal?>.Fail($"Requested average for range {startDate} - {endDate} older than 3 months");
            }

            if (startDate > today || endDate > today)
            {
                _logger.LogWarning($"Requested average for range {startDate} - {endDate} includes future dates");
                return ValidationResult<decimal?>.Fail($"Requested average for range {startDate} - {endDate} includes future dates");
            }

            if (startDate > endDate)
            {
                _logger.LogWarning($"Invalid range: start {startDate} is after end {endDate}");
                return ValidationResult<decimal?>.Fail($"Invalid range: start {startDate} is after end {endDate}");
            }

            var avg = await _rateRepository.GetAvarageAmountByRangeAsync(startDate, endDate, ct);
            return ValidationResult<decimal?>.Success(avg);
        }
    }
}
