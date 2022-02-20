using PaymentGateway.Library.Models.Settings;

namespace PaymentGateway.BankSimulator.Models.Settings
{
    public class BankSimulatorAppSettings
    {
        public string ClientRepositoryFilePath { get; set; }

        public KeyVaultAppSettings KeyVault { get; set; }

        public TableStorageAppSettings TableStorage { get; set; }
    }
}
