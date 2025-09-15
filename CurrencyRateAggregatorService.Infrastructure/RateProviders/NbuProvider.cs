using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Models.Providers;
using CurrencyRateAggregatorService.Infrastructure.Mappings;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;


namespace CurrencyRateAggregatorService.Infrastructure.RateProviders
{
    public class NbuProvider : IRateProvider
    {
        private HttpClient _httpClient;
        private readonly ILogger<NbuProvider> _logger;

        public string Name => "Nbu";

        public NbuProvider(HttpClient client, ILogger<NbuProvider> logger) 
        {
            _logger = logger;
            _httpClient = client;
        }

        public async Task<ProviderQuote?> GetRateByDateAsync(DateOnly date, CancellationToken ct)
        {
            var uri = BuildQuery(date);

            return await ExecuteWithRetryAsync(
                async () =>
                {
                    var response = await _httpClient.GetAsync(uri, ct);

                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"NBU responded with {response.StatusCode}");

                    var dtoList = await response.Content
                        .ReadFromJsonAsync<List<NbuRateDto>>(cancellationToken: ct);

                    if (dtoList == null || dtoList.Count == 0)
                        return null;

                    return dtoList[0].ToProviderQuote();
                },
                date,
                ct
            );
        }

        public async Task<IReadOnlyCollection<ProviderQuote>?> GetRangeOfRatesAsync(DateOnly startDate, DateOnly endDate,CancellationToken ct)
        {
            var uri = BuildRangeQuery(startDate, endDate);

            return await ExecuteWithRetryAsync(
                async () =>
                {
                    var response = await _httpClient.GetAsync(uri, ct);

                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"NBU responded with {response.StatusCode}");

                    var dtoList = await response.Content
                        .ReadFromJsonAsync<List<NbuRateDto>>(cancellationToken: ct);

                    if (dtoList == null || dtoList.Count == 0)
                        return null;

                    return dtoList
                        .Select(d => d.ToProviderQuote())
                        .OrderBy(q => q.ExchangeDate) 
                        .ToList();
                },
                startDate,
                ct
            );
        }

        private Uri BuildQuery (DateOnly date)
        {
            string targetDate = date.ToString("yyyyMMdd");

            var builder = new UriBuilder(_httpClient.BaseAddress!)
            {
                Path = "/NBUStatService/v1/statdirectory/exchangenew",
                Query = $"valcode=USD&date={targetDate}&json"
            };

            return builder.Uri;
        }

        private Uri BuildRangeQuery(DateOnly startDate, DateOnly endDate)
        {
            string targetStartDate = startDate.ToString("yyyyMMdd");
            string targetEndDate = endDate.ToString("yyyyMMdd");

            var builder = new UriBuilder(_httpClient.BaseAddress!)
            {
                Path = "/NBU_Exchange/exchange_site",
                Query = $"start={targetStartDate}&end={targetEndDate}&valcode=usd&sort=exchangedate&order=desc&json"
            };

            return builder.Uri;
        }

        private async Task<T?> ExecuteWithRetryAsync<T>(
            Func<Task<T?>> action,
            DateOnly date,
            CancellationToken ct,
            int maxRetries = 3,
            int delayMs = 500)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Request to NBU timed out for {Date} (attempt {Attempt}/{Max})",
                        date, attempt, maxRetries);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "HTTP error while requesting NBU for {Date} (attempt {Attempt}/{Max})",
                        date, attempt, maxRetries);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse NBU response for {Date}", date);
                    throw;
                }

                if (attempt < maxRetries)
                    await Task.Delay(delayMs, ct);
            }

            return default;
        }
    }
}
