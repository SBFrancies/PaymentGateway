using PaymentGateway.Library.Models.Settings;

namespace PaymentGateway.Merchant.Models.Settings
{
    public class MerchantAppSettings
    {
        public PaymentGatewayAppSettings PaymentGateway { get; set; }

        public AzureAdB2CAppSettings AzureAdB2C { get; set; }

        public TableStorageAppSettings TableStorage { get; set; }

    }
}
