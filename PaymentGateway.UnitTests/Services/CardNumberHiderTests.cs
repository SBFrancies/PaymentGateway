using PaymentGateway.Api.Services;
using Xunit;

namespace PaymentGateway.UnitTests.Services
{
    public class CardNumberHiderTests
    {
        [Theory]
        [InlineData("1111-2222-3333-4444", "XXXX-XXXX-XXXX-4444")]
        [InlineData("1234", "X234")]
        [InlineData("1", "X")]
        [InlineData("1234-5678", "XXXX-5678")]
        [InlineData("12345678", "XXXX5678")]
        public void CardNumberHider_ObscureCardNumber_MatchesExpectedResult(string cardNumber, string obscured)
        {
            var sut = GetSystemUnderTest();

            var result = sut.ObscureCardNumber(cardNumber);

            Assert.Equal(obscured, result);
        }


        private CardNumberHider GetSystemUnderTest()
        {
            return new CardNumberHider();
        }
    }
}

