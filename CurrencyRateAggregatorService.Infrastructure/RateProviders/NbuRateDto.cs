using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyRateAggregatorService.Infrastructure.RateProviders
{
    public class NbuRateDto
    {
        [JsonPropertyName("exchangedate")]
        public string ExchangeDate { get; set; } = string.Empty;

        [JsonPropertyName("r030")]
        public int R030 { get; set; }

        [JsonPropertyName("txt")]
        public string txt { get; set; } = string.Empty;

        [JsonPropertyName("cc")]
        public string Cc { get; set; } = string.Empty;

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; } 
    }
}
