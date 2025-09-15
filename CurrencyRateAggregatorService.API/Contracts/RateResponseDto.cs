namespace CurrencyRateAggregatorService.API.DTO
{
    public sealed class RateResponseDto
    {
        public DateOnly Date { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public string QuoteCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }
}
