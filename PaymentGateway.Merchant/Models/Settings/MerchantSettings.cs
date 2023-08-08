using PaymentGateway.Library.Models.Settings;
using System;

namespace PaymentGateway.Merchant.Models.Settings
{
    public class MerchantSettings
    {
        public MerchantSettings(MerchantAppSettings appsettings)
        {
            if(appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if (appsettings.TableStorage != null)
            {
                TableStorage = new TableStorageSettings(appsettings.TableStorage);
            }

            PaymentGateway = new PaymentGatewaySettings(appsettings.PaymentGateway);
            AzureAdB2C = new AzureAdB2CSettings(appsettings.AzureAdB2C);
        }

        public PaymentGatewaySettings PaymentGateway { get; }

        public AzureAdB2CSettings AzureAdB2C { get; }

        public TableStorageSettings TableStorage { get; }
    }
}
