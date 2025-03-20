using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;

using Polly;
using Polly.Retry;

namespace PaymentGateway.Infrastructure.Services
{
    public class BankService : IBankService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BankService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public BankService(HttpClient httpClient, ILogger<BankService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout || !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context)
                => _logger.LogWarning("Retry {RetryCount} due to {ExceptionType}. Retrying in {TimeSpanSeconds} seconds.",
                        retryCount, exception.GetType().Name, timeSpan.TotalSeconds));
        }

        public async Task<BankResponse> ProcessPaymentAsync(BankRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment for card ending with: {CardNumber}", request.CardNumber[^4..]);
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync("payments", request, cancellationToken));

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Bank returned status code: {response.StatusCode}");
                }

                var bankResponse = await response.Content.ReadFromJsonAsync<BankResponse>();
                return bankResponse ?? throw new InvalidOperationException("Bank response is null.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}