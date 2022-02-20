namespace PaymentGateway.Api.Interface
{
    public interface ICardNumberHider
    {
        string ObscureCardNumber(string cardNumber);
    }
}
