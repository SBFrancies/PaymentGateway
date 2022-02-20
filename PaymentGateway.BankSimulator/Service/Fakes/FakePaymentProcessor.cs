using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using PaymentGateway.BankSimulator.Interface;
using PaymentGateway.BankSimulator.Models.Request;
using PaymentGateway.BankSimulator.Models.Response;
using PaymentGateway.BankSimulator.Models.Settings;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Service.Fakes
{
    public class FakePaymentProcessor : IPaymentProcessor
    {
        private const int MaxValue = 10000;
        private const int MaxConcurrent = 10;
        private int _paymentCount = 0;
        private Random RandomNumGenerator { get; } = new Random();
        private IBankCodeGenerator BankCodeGenerator { get; }
        private ISystemClock Clock { get; }
        private ILogger<FakePaymentProcessor> Logger { get; }

        public FakePaymentProcessor(IBankCodeGenerator bankCodeGenerator, ISystemClock clock, ILogger<FakePaymentProcessor> logger)
        {
            BankCodeGenerator = bankCodeGenerator ?? throw new ArgumentNullException(nameof(bankCodeGenerator));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(HttpStatusCode code, CardPaymentResponse response)> ProcessPaymentAsync(CardPaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
            {
                throw new ArgumentNullException(nameof(paymentRequest));
            }
            try
            {
                if(Interlocked.Increment(ref _paymentCount) > MaxConcurrent)
                {
                    Logger.LogWarning("Too many concurrent payment requests");
                    return (HttpStatusCode.TooManyRequests, CardPaymentResponse.FromError("Payment request failed, server limit reached"));
                }

                if (paymentRequest.Amount <= 0)
                {
                    Logger.LogWarning("Payment request amount 0 or less");
                    return (HttpStatusCode.UnprocessableEntity, CardPaymentResponse.FromError("Payment request amount 0 or less"));
                }

                if (paymentRequest.Amount > MaxValue)
                {
                    Logger.LogWarning($"Payment request exceeds max value of {MaxValue}");
                    return (HttpStatusCode.BadRequest, CardPaymentResponse.FromError($"Payment request exceeds max value of {MaxValue} {paymentRequest.CurrencyCode}"));
                }

                // Fake processing with random wait
                var processingPeriod = RandomNumGenerator.Next(1000, 5000);
                await Task.Delay(TimeSpan.FromMilliseconds(processingPeriod));
                Logger.LogInformation($"Processing took {processingPeriod} milliseconds");
                var dateProcessed = Clock.UtcNow.DateTime;

                var bankCode = BankCodeGenerator.GenerateBankCode();
                Logger.LogInformation($"Generated bank code: {bankCode}");

                return (HttpStatusCode.OK, CardPaymentResponse.FromSuccess(bankCode, dateProcessed));
            }

            finally
            {
                Interlocked.Decrement(ref _paymentCount);
            }
        }
    }
}
