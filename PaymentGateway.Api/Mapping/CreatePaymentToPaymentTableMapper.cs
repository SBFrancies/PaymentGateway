using Microsoft.AspNetCore.Http;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Library.Mapping;
using System;

namespace PaymentGateway.Api.Mapping
{
    public class CreatePaymentToPaymentTableMapper : BaseMapper<CreatePaymentRequest, PaymentTable>
    {
        private IHttpContextAccessor HttpContextAccessor {get;}

        public CreatePaymentToPaymentTableMapper(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public override PaymentTable Map(CreatePaymentRequest item)
        {
            if(item == null)
            {
                return null;
            }

            return new PaymentTable
            {
                Amount = item.Amount,
                CardName = item.CardName,
                CardNumber = item.CardNumber,
                CurrencyCode = item.CurrencyCode,
                ExpiryMonth = (byte)item.ExpiryMonth,
                ExpiryYear = (short)item.ExpiryYear,
                Reference = item.Reference,
                CreatedBy = HttpContextAccessor.HttpContext.User.Identity.Name,
            };
        }
    }
}
