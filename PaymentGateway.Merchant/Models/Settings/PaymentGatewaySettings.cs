using PaymentGateway.Library.Exceptions;
using System;

namespace PaymentGateway.Merchant.Models.Settings
{
    public class PaymentGatewaySettings
    {
        public PaymentGatewaySettings(PaymentGatewayAppSettings appsettings)
        {
            if(appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if(!Uri.IsWellFormedUriString(appsettings.BaseUrl, UriKind.Absolute))
            {
                throw new InvalidConfigurationException(nameof(appsettings.BaseUrl), "BaseUrl must be a well formed absolute Uri");
            }

            BaseUrl = new Uri(appsettings.BaseUrl);

        }


        public Uri BaseUrl { get; }
    }
}
