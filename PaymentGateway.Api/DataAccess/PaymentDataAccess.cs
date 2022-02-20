using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Data;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.DataAccess
{
    public class PaymentDataAccess : IPaymentDataAccess
    {
        private Func<PaymentApiDbContext> ContextFactory { get; }

        public PaymentDataAccess(Func<PaymentApiDbContext> contextFactory)
        {
            ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<PaymentTable> SaveNewPaymentAsync(PaymentTable payment)
        {
            await using var context = ContextFactory();

            context.Add(payment);

            await context.SaveChangesAsync();

            return payment;
        }

        public async Task<PaymentTable> UpdatePaymentAsync(Guid paymentId, string bankCode)
        {
            await using var context = ContextFactory();

            var payment = await context.Payments.FirstOrDefaultAsync();

            if(payment == null)
            {
                throw new Exception("Payment not found");
            }

            payment.BankCode = bankCode;

            await context.SaveChangesAsync();

            return payment;
        }

        public async Task<IEnumerable<PaymentTable>> GetPaymentsAsync(string cardNumber, string reference)
        {
            await using var context = ContextFactory();

            var payments = context.Payments.AsQueryable();

            if(!string.IsNullOrEmpty(cardNumber))
            {
                payments = payments.Where(a => EF.Functions.Like(a.CardNumber, $"%{cardNumber}%"));
            }

            if (!string.IsNullOrEmpty(reference))
            {
                payments = payments.Where(a => EF.Functions.Like(a.Reference, $"%{reference}%"));
            }

            return await payments.ToListAsync();
        }

        public async Task<PaymentTable> GetPaymentAsync(Guid id)
        {
            await using var context = ContextFactory();

            var payment = await context.Payments.FirstOrDefaultAsync(a => a.Id == id);

            return payment;
        }
    }
}
