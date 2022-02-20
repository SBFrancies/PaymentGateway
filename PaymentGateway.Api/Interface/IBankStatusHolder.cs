namespace PaymentGateway.Api.Interface
{
    public interface IBankStatusHolder
    {
        void SetIsHealthy(bool healthy);

        bool IsHealthy { get; }
    }
}
