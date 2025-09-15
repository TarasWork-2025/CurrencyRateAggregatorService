using CurrencyRateAggregatorService.Infrastructure.RateProviders;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text;
using System.Text.Json;


namespace CurrencyRateAggregatorService.Tests.Infrastructure
{
    public class NbuProviderTests
    {
        private static HttpClient CreateHttpClient(HttpResponseMessage response)
        {
            return new HttpClient(new FakeHttpMessageHandler(response))
            {
                BaseAddress = new Uri("https://fake.test")
            };
        }

        [Fact]
        public async Task GetRateByDateAsync_Should_Return_ProviderQuote_When_ValidJson()
        {
            // Init
            var dto = new NbuRateDto
            {
                Cc = "USD",
                Rate = 36.6m,
                ExchangeDate = "12.09.2025"
            };

            var json = JsonSerializer.Serialize(new List<NbuRateDto> { dto });
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var httpClient = CreateHttpClient(response);
            var provider = new NbuProvider(httpClient, NullLogger<NbuProvider>.Instance);

            // Action
            var result = await provider.GetRateByDateAsync(DateOnly.Parse("2025-09-12"), default);

            // Result
            Assert.NotNull(result);
            Assert.Equal("USD", result.BaseCurrency);
            Assert.Equal("UAH", result.QuoteCurrency);
            Assert.Equal(36.6m, result.Amount);
        }

        [Fact]
        public async Task GetRateByDateAsync_Should_Return_Null_When_EmptyList()
        {
            // Init
            var json = JsonSerializer.Serialize(new List<NbuRateDto>());
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var httpClient = CreateHttpClient(response);
            var provider = new NbuProvider(httpClient, NullLogger<NbuProvider>.Instance);

            // Action
            var result = await provider.GetRateByDateAsync(DateOnly.Parse("2025-09-12"), default);

            // Result
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRateByDateAsync_Should_Throw_On_HttpError()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var httpClient = CreateHttpClient(response);
            var provider = new NbuProvider(httpClient, NullLogger<NbuProvider>.Instance);

            // Action + Assert
            var result = await provider.GetRateByDateAsync(DateOnly.Parse("2025-09-12"), default);
            Assert.Null(result);
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
