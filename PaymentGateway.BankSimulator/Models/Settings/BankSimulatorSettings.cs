using PaymentGateway.Library.Exceptions;
using PaymentGateway.Library.Models.Settings;
using System;

namespace PaymentGateway.BankSimulator.Models.Settings
{
    public class BankSimulatorSettings
    {
        public BankSimulatorSettings(BankSimulatorAppSettings appsettings)
        {
            if (appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if (string.IsNullOrEmpty(appsettings.ClientRepositoryFilePath))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ClientRepositoryFilePath), "ClientRepositoryFilePath cannot be null or empty");
            }

            ClientRepositoryFilePath = appsettings.ClientRepositoryFilePath;
            KeyVault = new KeyVaultSettings(appsettings.KeyVault);
            TableStorage = new TableStorageSettings(appsettings.TableStorage);
        }

        public string ClientRepositoryFilePath { get; }

        public KeyVaultSettings KeyVault { get; }

        public TableStorageSettings TableStorage { get; }
    }
}
