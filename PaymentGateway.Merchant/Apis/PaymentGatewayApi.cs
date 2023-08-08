using Microsoft.Identity.Web;
using PaymentGateway.Library.Serialisation;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.ApiModels;
using PaymentGateway.Merchant.Models.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Apis
{
    public class PaymentGatewayApi : IGatewayApi
    {
        private ITokenAcquisition TokenAcquisition { get; }
        private HttpClient HttpClient { get; }
        private MerchantSettings Settings { get; }

        public PaymentGatewayApi(ITokenAcquisition tokenAcquisition, IHttpClientFactory clientFactory, MerchantSettings settings)
        {
            TokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
            HttpClient = clientFactory?.CreateClient("PaymentGateway") ?? throw new ArgumentNullException(nameof(clientFactory));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<PaymentResponse> GetAsync(Guid id)
        {
            var accessToken = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { Settings.AzureAdB2C.AccessTokenScope });
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await HttpClient.GetAsync($"Payment/{id}");

            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var payment = JsonSerializer.Deserialize<PaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);
                return payment;
            }

            catch 
            {
                throw new Exception(responseBody);
            }
        }

        public async Task<NewPaymentResponse> PostAsync(CreatePayment createPayment)
        {
            var accessToken = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { Settings.AzureAdB2C.AccessTokenScope });
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var message = new HttpRequestMessage(HttpMethod.Post, "Payment")
            {
                Content = new JsonContent(createPayment),
            };

            var response = await HttpClient.SendAsync(message);
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var payment = JsonSerializer.Deserialize<NewPaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);
                return payment;
            }

            catch
            {
                throw new Exception(responseBody);
            }
        }

        public async Task<IEnumerable<Payment>> GetAsync(string cardNumber, string reference)
        {
            var accessToken = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { Settings.AzureAdB2C.AccessTokenScope });
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await HttpClient.GetAsync($"Payment?cardNumber={cardNumber}&reference={reference}");
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var payments = JsonSerializer.Deserialize<PaymentsResponse>(responseBody, SerialisationSettings.DefaultOptions);
                return payments.Payments;
            }

            catch
            {
                throw new Exception(responseBody);
            }
        }
    }
}
