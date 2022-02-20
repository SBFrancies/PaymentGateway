using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using PaymentGateway.Api.Validation;

namespace PaymentGateway.UnitTests.Validation
{
    public class PaymentValidatorTests
    {
        private readonly Mock<ISystemClock> _mockClock = new Mock<ISystemClock>();


        private PaymentValidator GetSystemUnderTest()
        {
            return new PaymentValidator(_mockClock.Object);
        }
    }
}
