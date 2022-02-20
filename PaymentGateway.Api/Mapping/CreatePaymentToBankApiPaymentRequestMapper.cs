using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Library.Mapping;

namespace PaymentGateway.Api.Mapping
{
    public class CreatePaymentToBankApiPaymentRequestMapper : BaseMapper<CreatePaymentRequest, CardPaymentRequest>
    {
        public override CardPaymentRequest Map(CreatePaymentRequest item)
        {
            if (item == null)
            {
                return null;
            }

            return new CardPaymentRequest
            {
                Amount = item.Amount,
                CardName = item.CardName,
                CardNumber = item.CardNumber,
                CurrencyCode = item.CurrencyCode,
                Cvv = item.Cvv,
                ExpiryMonth = item.ExpiryMonth,
                ExpiryYear = item.ExpiryYear,
                Reference = item.Reference,
            };
        }
    }
}
