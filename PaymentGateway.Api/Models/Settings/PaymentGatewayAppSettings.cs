using PaymentGateway.Library.Models.Settings;

namespace PaymentGateway.Api.Models.Settings
{
    public class PaymentGatewayAppSettings
    {
        public KeyVaultAppSettings KeyVault { get; set; }

        public TableStorageAppSettings TableStorage { get; set; }

        public BankApiAppSettings Bank { get; set; }

        public AuthenticationAppSettings Authentication { get; set; }

        public string EventStoreConnectionString { get; set; }

        public string SqlDbConnectionString { get; set; }
    }
}
