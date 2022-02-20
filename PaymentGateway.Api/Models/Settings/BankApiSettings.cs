using PaymentGateway.Library.Exceptions;
using System;

namespace PaymentGateway.Api.Models.Settings
{
    public class BankApiSettings
    {
        public BankApiSettings(BankApiAppSettings appsettings)
        {
            if (appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if (string.IsNullOrEmpty(appsettings.ApiKey))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ApiKey), "Bank:ApiKey cannot be null or empty");
            }

            if(!Uri.IsWellFormedUriString(appsettings.BaseUrl, UriKind.Absolute))
            {
                throw new InvalidConfigurationException(nameof(appsettings.BaseUrl), "Bank:BaseUrl mut be a well formed absolute Uri");
            }

            ApiKey = appsettings.ApiKey;
            BaseUrl = new Uri(appsettings.BaseUrl);
        }

        public Uri BaseUrl { get; }

        public string ApiKey { get;  }
    }
}
