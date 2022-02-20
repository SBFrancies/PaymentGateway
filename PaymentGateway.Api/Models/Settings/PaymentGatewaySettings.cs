using PaymentGateway.Api.Interface;
using PaymentGateway.Library.Exceptions;
using PaymentGateway.Library.Models.Settings;
using System;

namespace PaymentGateway.Api.Models.Settings
{
    public class PaymentGatewaySettings
    {
        public PaymentGatewaySettings(PaymentGatewayAppSettings appsettings)
        {
            if(appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if(string.IsNullOrEmpty(appsettings.EventStoreConnectionString))
            {
                throw new InvalidConfigurationException(nameof(appsettings.EventStoreConnectionString), "EventStoreConnectionString cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.SqlDbConnectionString))
            {
                throw new InvalidConfigurationException(nameof(appsettings.SqlDbConnectionString), "SqlDbConnectionString cannot be null or empty");
            }

            KeyVault = new KeyVaultSettings(appsettings.KeyVault);

            if (appsettings.TableStorage != null)
            {
                TableStorage = new TableStorageSettings(appsettings.TableStorage);
            }

            Bank = new BankApiSettings(appsettings.Bank);
            Authentication = new AuthenticationSettings(appsettings.Authentication);
            EventStoreConnectionString = appsettings.EventStoreConnectionString;
            SqlDbConnectionString = appsettings.SqlDbConnectionString;
        }

        public KeyVaultSettings KeyVault { get; }

        public TableStorageSettings TableStorage { get; }

        public BankApiSettings Bank { get; }

        public AuthenticationSettings Authentication { get; }

        public string EventStoreConnectionString { get; }

        public string SqlDbConnectionString { get; }
    }
}
