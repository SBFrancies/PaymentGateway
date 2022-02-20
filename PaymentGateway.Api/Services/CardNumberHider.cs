using PaymentGateway.Api.Interface;
using System;
using System.Text.RegularExpressions;

namespace PaymentGateway.Api.Services
{
    public class CardNumberHider : ICardNumberHider
    {
        public string ObscureCardNumber(string cardNumber)
        {
            if(cardNumber == null)
            {
                throw new ArgumentNullException(nameof(cardNumber));
            }

            var pattern = "[0-9]";
            var matches = Regex.Matches(cardNumber, pattern);
            var numsToLeave = matches.Count - (matches.Count > 4 ? 4 : matches.Count - 1);
            var charArray = cardNumber.ToCharArray();

            for(var i = 0; i < numsToLeave; i++)
            {
                charArray[matches[i].Index] = 'X';
            }

            return new string(charArray);
        }
    }
}
