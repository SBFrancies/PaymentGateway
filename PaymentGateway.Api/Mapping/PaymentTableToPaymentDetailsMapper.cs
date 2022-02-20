
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Library.Mapping;
using System;

namespace PaymentGateway.Api.Mapping
{
    public class PaymentTableToPaymentDetailsMapper : BaseMapper<PaymentTable, PaymentDetails>
    {
        ICardNumberHider CardNumberHider { get; }

        public PaymentTableToPaymentDetailsMapper(ICardNumberHider cardNumberHider)
        {
            CardNumberHider = cardNumberHider ?? throw new ArgumentNullException(nameof(cardNumberHider));
        }

        public override PaymentDetails Map(PaymentTable item)
        {
            if(item == null)
            {
                return null;
            }

            return new PaymentDetails
            {
                Amount = item.Amount,
                BankCode = item.BankCode,
                CardName = item.CardName,
                CardNumber = CardNumberHider.ObscureCardNumber(item.CardNumber),
                CurrencyCode = item.CurrencyCode,
                ExpiryMonth = item.ExpiryMonth,
                ExpiryYear = item.ExpiryYear,
                Id = item.Id,   
                Reference = item.Reference,
            };
        }
    }
}
