using CurrencyRateAggregatorService.Application.Models.Providers;
using CurrencyRateAggregatorService.Domain;


namespace CurrencyRateAggregatorService.Application.Mappings
{
    public static class ApplicationMappingExtensions
    {
        public static Rate? ToRate (this ProviderQuote quote)
        {
            if(quote != null)
            {
                return new Rate(Guid.NewGuid(),quote.ExchangeDate, quote.BaseCurrency, quote.QuoteCurrency, quote.Unit, quote.Amount);
            }
            return null;
        }
    }
}
