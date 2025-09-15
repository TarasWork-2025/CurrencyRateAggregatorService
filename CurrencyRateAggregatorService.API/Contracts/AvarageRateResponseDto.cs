namespace CurrencyRateAggregatorService.API.DTO
{
    public sealed class AverageRateResponseDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Average { get; set; }
    }
}
