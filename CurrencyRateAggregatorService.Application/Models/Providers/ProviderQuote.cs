namespace CurrencyRateAggregatorService.Application.Models.Providers
{
    public class ProviderQuote
    {
        public DateOnly ExchangeDate { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public int Unit { get; set; }
        public decimal Amount { get; set; }

    }
}
