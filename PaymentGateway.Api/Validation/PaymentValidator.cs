using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using PaymentGateway.Api.Models.Request;

namespace PaymentGateway.Api.Validation
{
    public class PaymentValidator : AbstractValidator<CreatePaymentRequest>
    {
        public PaymentValidator(ISystemClock clock)
        {
            RuleFor(a => a.CardName).NotNull().NotEmpty().MaximumLength(100);
            RuleFor(a => a.Reference).MaximumLength(50);
            RuleFor(a => a.ExpiryMonth).GreaterThan(0).LessThanOrEqualTo(12);
            RuleFor(a => a.ExpiryYear).GreaterThanOrEqualTo(clock.UtcNow.Year);
            RuleFor(a => a.ExpiryMonth).GreaterThanOrEqualTo(clock.UtcNow.Month).When(a => a.ExpiryYear == clock.UtcNow.Year);
            RuleFor(a => a.Cvv).NotNull().MinimumLength(3).MaximumLength(4).Matches("^[0-9]{3,4}$");
            RuleFor(a => a.CurrencyCode).NotNull().MinimumLength(3).MaximumLength(3).Matches("^[a-zA-Z]{3}$");
            RuleFor(a => a.Amount).GreaterThan(0).LessThan(10000000000);
            RuleFor(a => a.CardNumber).NotNull().NotEmpty().Matches("^([0-9]+([\\s-][0-9]+)*)+$").MaximumLength(30);
        }
    }
}
