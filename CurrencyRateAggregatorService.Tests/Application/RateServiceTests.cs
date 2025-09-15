using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Models.Providers;
using CurrencyRateAggregatorService.Application.Services;
using CurrencyRateAggregatorService.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CurrencyRateAggregatorService.Tests.Application
{
    public class RateServiceTests
    {
        private readonly Mock<IRateRepository> _repoMock = new();
        private readonly Mock<IRateProvider> _providerMock = new();

        private RateService CreateService() =>
            new RateService(_providerMock.Object, _repoMock.Object, NullLogger<RateService>.Instance);

        [Fact]
        public async Task GetRateByDateAsync_Should_Return_From_Repository_When_Exists()
        {
            // Init
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var expected = new Rate(Guid.NewGuid(), date, "USD", "UAH", 1, 36.6m);

            _repoMock.Setup(r => r.GetRateByDateAsync(date, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var service = CreateService();

            // Action
            var result = await service.GetRateByDateAsync(date, default);

            // Check result
            Assert.True(result.IsValid);
            Assert.NotNull(result.Value);
            Assert.Equal(expected, result.Value);
            _providerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetRateByDateAsync_Should_Call_Provider_When_Not_In_Repository()
        {
            // Init
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            _repoMock.Setup(r => r.GetRateByDateAsync(date, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Rate?)null);

            var quote = new ProviderQuote
            {
                BaseCurrency = "USD",
                QuoteCurrency = "UAH",
                Unit = 1,
                Amount = 36.6m,
                ExchangeDate = date
            };

            _providerMock.Setup(p => p.GetRateByDateAsync(date, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var service = CreateService();

            // Acttion
            var result = await service.GetRateByDateAsync(date, default);

            // Check result
            Assert.True(result.IsValid);
            Assert.NotNull(result.Value);
            Assert.Equal("USD", result.Value!.BaseCurrency);
            Assert.Equal("UAH", result.Value.QuoteCurrency);
        }

        [Fact]
        public async Task GetAverageRateAsync_Should_Return_Value_From_Repository()
        {
            // Init
            var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));
            var end = DateOnly.FromDateTime(DateTime.UtcNow);

            _repoMock.Setup(r => r.GetAvarageAmountByRangeAsync(start, end, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(37m);

            var service = CreateService();

            // Acttion
            var result = await service.GetAverageRateAsync(start, end, default);

            // Chech result
            Assert.True(result.IsValid);
            Assert.True(result.Value.HasValue);
            Assert.Equal(37m, result.Value.Value);
        }
    }
}
