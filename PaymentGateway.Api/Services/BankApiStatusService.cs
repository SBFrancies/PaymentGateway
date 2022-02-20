using PaymentGateway.Api.Interface;

namespace PaymentGateway.Api.Services
{
    public class BankApiStatusService : IBankStatusHolder
    {
        public bool IsHealthy { get; private set; }

        public void SetIsHealthy(bool healthy)
        {
            IsHealthy = healthy;
        }
    }
}
