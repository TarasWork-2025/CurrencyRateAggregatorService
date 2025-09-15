using CurrencyRateAggregatorService.Application.Models.Providers;
using CurrencyRateAggregatorService.Infrastructure.RateProviders;
using System.Globalization;


namespace CurrencyRateAggregatorService.Infrastructure.Mappings
{
    public static class InfrastructureMappingExtensions
    {
        public static ProviderQuote ToProviderQuote(this NbuRateDto nbuRateDto)
        {
            ProviderQuote quote = new ProviderQuote()
            {
                BaseCurrency = nbuRateDto.Cc,
                QuoteCurrency = "UAH",
                Unit = 1,
                Amount = nbuRateDto.Rate,
                ExchangeDate = DateOnly.ParseExact(nbuRateDto.ExchangeDate, "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            return quote;
        }
    }
}
