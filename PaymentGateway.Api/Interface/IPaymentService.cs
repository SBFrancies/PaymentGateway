using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Interface
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);

        Task<FetchPaymentsResponse> FetchPaymentsAsync(FetchPaymentsRequest request);

        Task<PaymentResponse> FetchPaymentAsync(Guid id);
    }
}