namespace CurrencyRateAggregatorService.Domain
{
    public class Rate
    {
        public Guid Id { get; private set; }
        public DateOnly Date { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public int Units { get; private set; }
        public decimal Amount { get; private set; }
        public decimal RatePer1 => Amount / Units;

        private Rate() { }

        public Rate(Guid id, DateOnly date, string baseCurrency, string quoteCurrency, int units, decimal amount)
        {
            Id = id;
            if (units <= 0) throw new ArgumentOutOfRangeException(nameof(units));
            if (string.IsNullOrWhiteSpace(baseCurrency)) throw new ArgumentException("Base currency required");
            if (string.IsNullOrWhiteSpace(quoteCurrency)) throw new ArgumentException("Quote currency required");
            Date = date;
            Amount = amount;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Units = units;
        }

        public void UpdateQuoteCurrency (string quoteCurrency)
        {
            if (string.IsNullOrWhiteSpace(quoteCurrency)) throw new ArgumentException("Quote currency required");
            this.QuoteCurrency = quoteCurrency;
        }

        public void UpdateBaseCurrency (string baseCurrency)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency)) throw new ArgumentException("Base currency required");
            this.BaseCurrency = baseCurrency;
        }

        public void UpdateUnits (int units)
        {
            Units = units;
        }

        public void UpdateAmount(decimal amount)
        {
            Amount = amount;
        }
    }
}
