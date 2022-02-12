using Microsoft.Identity.Web;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Apis
{
    public class PaymentGatewayApi : IGatewayApi
    {
        private ITokenAcquisition TokenAcquisition { get; }

        public PaymentGatewayApi(ITokenAcquisition tokenAcquisition)
        {
            TokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
        }

        public async Task GetAsync(Guid id)
        {
            var client = new HttpClient();
            var accessToken = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://PaymentGatewayAD.onmicrosoft.com/f5fd2651-c11f-490b-9e81-f3933aa7a0ac/Payments.ReadWrite" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"https://localhost:5002/Payment/{Guid.NewGuid()}");
        }

        public Task<IEnumerable<Payment>> GetAsync(string cardNumber, string reference)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> PostAsync(CreatePayment createPayment)
        {
            throw new NotImplementedException();
        }
    }
}
