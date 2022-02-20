using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.IntegrationTests.FakeServices;
using PaymentGateway.Library.Serialisation;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.IntegrationTests.Controllers
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task PaymentController_GetAsync_NotFoundWhenNoPayments()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeHappyBankApi>();

            var client = webContext.CreateDefaultClient();

            var result = await client.GetAsync("/Payment");

            Assert.NotNull(result);

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task PaymentController_GetAsync_WillReturnCreatedPayments()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeHappyBankApi>();

            var id = await CreatePaymentAsync(webContext);

            var client = webContext.CreateDefaultClient();

            var result = await client.GetAsync("/Payment");
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<FetchPaymentsResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Single(response.Payments);
            Assert.Equal(id, response.Payments.Single().Id);
        }

        [Fact]
        public async Task PaymentController_GetAsync_WillReturnCreatedPayment()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeHappyBankApi>();

            var id = await CreatePaymentAsync(webContext);

            var client = webContext.CreateDefaultClient();

            var result = await client.GetAsync($"/Payment/{id}");
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(id, response.Payment.Id);
        }

        [Fact]
        public async Task PaymentController_PostAsync_WillReturnCreatedPaymentHappyBank()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeHappyBankApi>();

            var client = webContext.CreateDefaultClient();

            var request = new CreatePaymentRequest
            {
                Amount = 100.00m,
                CardName = "TEST_USER",
                CardNumber = "1234 5678 91011",
                CurrencyCode = "USD",
                Cvv = "000",
                ExpiryMonth = 11,
                ExpiryYear = 2040,
                Reference = "TESTING",
            };

            var content = new JsonContent(request);
            var result = await client.PostAsync("/Payment", content);
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CreatePaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.NotEqual(Guid.Empty, response.PaymentId);
            Assert.Equal(JsonSerializer.Serialize(request), JsonSerializer.Serialize(response.Request));
            Assert.True(response.Success);
        }

        [Fact]
        public async Task PaymentController_PostAsync_WillReturn422WhenInvalidHappyBank()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeHappyBankApi>();

            var client = webContext.CreateDefaultClient();

            var request = new CreatePaymentRequest
            {
                Amount = 100.00m,
                CardName = "TEST_USER",
                CardNumber = "1234 5678 91011",
                CurrencyCode = "USD",
                Cvv = "NOT_VALID",
                ExpiryMonth = 11,
                ExpiryYear = 2040,
                Reference = "TESTING",
            };

            var content = new JsonContent(request);
            var result = await client.PostAsync("/Payment", content);
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);;
            Assert.Equal("Validation failed", response.ErrorId);
        }

        [Fact]
        public async Task PaymentController_PostAsync_WillReturnBadRequestWhenUnhappyBank()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeUnhappyBankApi>();

            var client = webContext.CreateDefaultClient();

            var request = new CreatePaymentRequest
            {
                Amount = 100.00m,
                CardName = "TEST_USER",
                CardNumber = "1234 5678 91011",
                CurrencyCode = "USD",
                Cvv = "123",
                ExpiryMonth = 11,
                ExpiryYear = 2040,
                Reference = "TESTING",
            };

            var content = new JsonContent(request);
            var result = await client.PostAsync("/Payment", content);
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CreatePaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotEqual(Guid.Empty, response.PaymentId);
            Assert.Equal(JsonSerializer.Serialize(request), JsonSerializer.Serialize(response.Request));
            Assert.False(response.Success);
        }

        [Fact]
        public async Task PaymentController_PostAsync_WillReturnBadRequestWhenUnhealthyBank()
        {
            using var webContext = new PaymentGatewayWebApplicationFactory<FakeUnhealthyBankApi>();

            var client = webContext.CreateDefaultClient();

            var request = new CreatePaymentRequest
            {
                Amount = 100.00m,
                CardName = "TEST_USER",
                CardNumber = "1234 5678 91011",
                CurrencyCode = "USD",
                Cvv = "123",
                ExpiryMonth = 11,
                ExpiryYear = 2040,
                Reference = "TESTING",
            };

            var content = new JsonContent(request);
            var result = await client.PostAsync("/Payment", content);
            var responseBody = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CreatePaymentResponse>(responseBody, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(Guid.Empty, response.PaymentId);
            Assert.Equal("Bank not available, please try again later", response.Message);
            Assert.Equal(JsonSerializer.Serialize(request), JsonSerializer.Serialize(response.Request));
            Assert.False(response.Success);
        }

        private async Task<Guid> CreatePaymentAsync<T>(PaymentGatewayWebApplicationFactory<T> context, CreatePaymentRequest request = null) where T : class , IBankApi
        {
            var client = context.CreateDefaultClient();

            request ??= new CreatePaymentRequest
            {
                Amount = 100.00m,
                CardName = "TEST_USER",
                CardNumber = "1234 5678 91011",
                CurrencyCode = "USD",
                Cvv = "000",
                ExpiryMonth = 11,
                ExpiryYear = 2040,
                Reference = "TESTING",
            };

            var content = new JsonContent(request);

            var response = await client.PostAsync("/Payment", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CreatePaymentResponse>(responseBody, SerialisationSettings.DefaultOptions).PaymentId;
      }
    }
}
