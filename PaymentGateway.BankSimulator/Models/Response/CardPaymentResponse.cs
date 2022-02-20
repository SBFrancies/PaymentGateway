using System;

namespace PaymentGateway.BankSimulator.Models.Response
{
    public class CardPaymentResponse
    {
        private CardPaymentResponse()
        {

        }

        public bool Success { get; private set; }

        public DateTime? DateProcessed { get; private set; }

        public string Message { get; private set; }

        public static CardPaymentResponse FromError(string error)
        {
            return new CardPaymentResponse
            {
                Message = error,
            };
        }

        public static CardPaymentResponse FromSuccess(string bankCode, DateTime dateProcessed)
        {
            return new CardPaymentResponse
            {
                Success = true,
                DateProcessed = dateProcessed,
                Message = bankCode,
            };
        }
    }
}
