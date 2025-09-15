using CurrencyRateAggregatorService.Domain;
using System;

namespace CurrencyRateAggregatorService.Tests.Domain
{
    public class RateTests
    {
        [Fact]
        public void Constructor_Should_Create_Rate_When_Valid()
        {
            var id = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            var rate = new Rate(id,date, "USD", "UAH", 1, 36.6m);

            Assert.Equal(date, rate.Date);
            Assert.Equal("USD", rate.BaseCurrency);
            Assert.Equal("UAH", rate.QuoteCurrency);
            Assert.Equal(1, rate.Units);
            Assert.Equal(36.6m, rate.Amount);
            Assert.Equal(36.6m, rate.RatePer1);
        }

        [Fact]
        public void Constructor_Should_Throw_When_Units_Invalid()
        {
            var id = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Rate(id, date, "USD", "UAH", 0, 36.6m));
        }

        [Fact]
        public void Constructor_Should_Throw_When_BaseCurrency_Is_Empty()
        {
            var id = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            Assert.Throws<ArgumentException>(() =>
                new Rate(id,date, "", "UAH", 1, 36.6m));
        }

        [Fact]
        public void Constructor_Should_Throw_When_QuoteCurrency_Is_Empty()
        {
            var id = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.UtcNow);

            Assert.Throws<ArgumentException>(() =>
                new Rate(id, date, "USD", "", 1, 36.6m));
        }

        [Fact]
        public void UpdateBaseCurrency_Should_Change_Value()
        {
            var rate = new Rate(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);

            rate.UpdateBaseCurrency("EUR");

            Assert.Equal("EUR", rate.BaseCurrency);
        }

        [Fact]
        public void UpdateQuoteCurrency_Should_Change_Value()
        {
            var rate = new Rate(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);

            rate.UpdateQuoteCurrency("GBP");

            Assert.Equal("GBP", rate.QuoteCurrency);
        }

        [Fact]
        public void UpdateUnits_Should_Change_Value()
        {
            var rate = new Rate(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);

            rate.UpdateUnits(10);

            Assert.Equal(10, rate.Units);
        }

        [Fact]
        public void UpdateAmount_Should_Change_Value()
        {
            var rate = new Rate(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 1, 36.6m);

            rate.UpdateAmount(40m);

            Assert.Equal(40m, rate.Amount);
        }

        [Fact]
        public void RatePer1_Should_Calculate_Correctly()
        {
            var rate = new Rate(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), "USD", "UAH", 2, 73.2m);

            Assert.Equal(36.6m, rate.RatePer1);
        }
    }
}
