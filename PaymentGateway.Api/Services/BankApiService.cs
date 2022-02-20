using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Settings;
using PaymentGateway.Library.Serialisation;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public class BankApiService : IBankApi
    {
        private const string ApiKeyHeader = "X-API-KEY";
        private HttpClient Client { get; }
        private ILogger<BankApiService> Logger { get; }
        private PaymentGatewaySettings Settings { get; }

        public BankApiService(IHttpClientFactory clientFactory, ILogger<BankApiService> logger, PaymentGatewaySettings settings)
        {
            Client = clientFactory?.CreateClient("BankApi") ?? throw new ArgumentNullException(nameof(clientFactory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<CardPaymentResponse> MakeBankPaymentAsync(CardPaymentRequest request)
        {
            var body = new JsonContent(request);
            var message = new HttpRequestMessage(HttpMethod.Post, "/CardPayment")
            {
                Content = body,
            };
            message.Headers.Add(ApiKeyHeader, Settings.Bank.ApiKey);

            var result = await Client.SendAsync(message);
            var responseBody = await result.Content.ReadAsStringAsync();

            try
            {
                var response = JsonSerializer.Deserialize<CardPaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);
                return response;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"Error getting response from Bank API, status code {result.StatusCode}, response: {responseBody}");
            }

            return null;
        }
        
        public async Task<bool> CheckBankHealthyAsync()
        {
            var response = await Client.GetAsync("/health");

            return response.IsSuccessStatusCode;
        }
    }
}
