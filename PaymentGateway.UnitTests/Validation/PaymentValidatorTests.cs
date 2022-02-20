using Moq;
using Microsoft.AspNetCore.Authentication;
using PaymentGateway.Api.Validation;
using Xunit;
using PaymentGateway.Api.Models.Request;
using System;

namespace PaymentGateway.UnitTests.Validation
{
    public class PaymentValidatorTests
    {
        private readonly Mock<ISystemClock> _mockClock = new Mock<ISystemClock>();

        [Fact]
        public void PaymentValidator_Validate_ValidPaymentIsValid()
        {
            var paymentRequest = GetValidCardPayment();

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PaymentValidator_Validate_CardNameCannotBeNullOrEmpty(string cardName)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CardName = cardName;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("fgdhejfbjfnjrfnjreur6uriruierueiriru28984820409390404hdjhdjendedjedjejdeury3ur3yrhh3jdjedjehjfefhjehefjefjrfrifrf857")]
        [InlineData("fgdhejfbjfnjrfnjreur6uriruierueiriru28984820409390404hdjhdjendedjedjejdeury3ur3yrhh3jdjedjehjfefhjehefjefjrfrifrf857VVVVVVVVVVVVVVVVVVVVVVVDdd")]
        public void PaymentValidator_Validate_CardNameCannotBeLongThan100Chars(string cardName)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CardName = cardName;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("fgdmdkfkdfjkjdfkjdf67890jjjkjnjedjededenjendedjedjejdeury3ur3yrhh3jdjedjehjfefhjehefjefjrfrifrf857")]
        [InlineData("dedjedjejdeury3ur3yrhh3jdjedjehjfefhjehefjefjrfrifrf857VVVVVVVVVVVVVVVVVVVVVVVDdd")]
        public void PaymentValidator_Validate_ReferenceCannotBeLongThan50Chars(string reference)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Reference = reference;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(66)]
        [InlineData(14)]
        [InlineData(-1)]
        [InlineData(0)]
        public void PaymentValidator_Validate_MonthMustBeValidMonth(int month)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.ExpiryMonth = month;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }


        [Theory]
        [InlineData(2020)]
        [InlineData(1999)]
        [InlineData(-1)]
        [InlineData(0)]
        public void PaymentValidator_Validate_YearCannotBeInPast(int year)
        {
            _mockClock.SetupGet(a => a.UtcNow).Returns(new DateTimeOffset(new DateTime(2022, 12, 1)));

            var paymentRequest = GetValidCardPayment();
            paymentRequest.ExpiryYear = year;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PaymentValidator_Validate_CvvCannotBeNullOrEmpty(string cvv)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Cvv = cvv;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("12")]
        [InlineData("22323")]
        public void PaymentValidator_Validate_CvvMustBe3Or4Characters(string cvv)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Cvv = cvv;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("1a2")]
        [InlineData("XXXX")]
        public void PaymentValidator_Validate_CvvMustBeNumeric(string cvv)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Cvv = cvv;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PaymentValidator_Validate_CurrencyCodeCannotBeNullOrEmpty(string code)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CurrencyCode = code;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("USDX")]
        public void PaymentValidator_Validate_CurrencyCodeMustBe3Characters(string code)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CurrencyCode = code;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("AB1")]
        [InlineData("!£$")]
        public void PaymentValidator_Validate_CurrencyCodeMustBeLetters(string code)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CurrencyCode = code;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-87238)]
        public void PaymentValidator_Validate_AmountMustBeGreaterThanZero(decimal amount)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Amount = amount;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(10000000000)]
        [InlineData(938643473635353)]
        public void PaymentValidator_Validate_AmountMustBeLessThan10000000000(decimal amount)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.Amount = amount;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PaymentValidator_Validate_CardNumberCannotBeNullOrEmpty(string cardNumber)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CardNumber = cardNumber;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }


        [Theory]
        [InlineData("23456-fghjk-344774-333")]
        [InlineData("-")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public void PaymentValidator_Validate_CardNumberMustMatchRegex(string cardNumber)
        {
            var paymentRequest = GetValidCardPayment();
            paymentRequest.CardNumber = cardNumber;

            var sut = GetSystemUnderTest();

            var result = sut.Validate(paymentRequest);

            Assert.False(result.IsValid);
        }

        private PaymentValidator GetSystemUnderTest()
        {
            return new PaymentValidator(_mockClock.Object);
        }

        private CreatePaymentRequest GetValidCardPayment()
        {
            var paymentRequest = new CreatePaymentRequest
            {
                CardName = "MR TEST",
                Amount = 100.00m,
                CardNumber = "1234 2345 5678 67845",
                CurrencyCode = "USD",
                Cvv = "123",
                ExpiryMonth = 1,
                ExpiryYear = 2039,
                Reference = "Reference",
            };

            return paymentRequest;
        }
    }
}
