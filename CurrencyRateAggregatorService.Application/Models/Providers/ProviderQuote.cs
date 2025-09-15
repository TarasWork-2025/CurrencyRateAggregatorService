using CurrencyRateAggregatorService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
