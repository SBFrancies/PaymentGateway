using PaymentGateway.Library.Exceptions;
using System;

namespace PaymentGateway.Merchant.Models.Settings
{
    public class AzureAdB2CSettings
    {
        public AzureAdB2CSettings(AzureAdB2CAppSettings appsettings)
        {
            if(appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if(string.IsNullOrEmpty(appsettings.Instance))
            {
                throw new InvalidConfigurationException(nameof(appsettings.Instance), "Instance cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.ClientId))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ClientId), "ClientId cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.Domain))
            {
                throw new InvalidConfigurationException(nameof(appsettings.Domain), "Domain cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.SignedOutCallbackPath))
            {
                throw new InvalidConfigurationException(nameof(appsettings.SignedOutCallbackPath), "SignedOutCallbackPath cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.SignUpSignInPolicyId))
            {
                throw new InvalidConfigurationException(nameof(appsettings.SignUpSignInPolicyId), "SignUpSignInPolicyId cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.ClientSecret))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ClientSecret), "ClientSecret cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.AccessTokenScope))
            {
                throw new InvalidConfigurationException(nameof(appsettings.AccessTokenScope), "AccessTokenScope cannot be null or empty");
            }

            Instance = appsettings.Instance;
            ClientId = appsettings.ClientId;
            Domain = appsettings.Domain;
            SignedOutCallbackPath = appsettings.SignedOutCallbackPath;
            SignUpSignInPolicyId = appsettings.SignUpSignInPolicyId;
            ClientSecret = appsettings.ClientSecret;
            AccessTokenScope = appsettings.AccessTokenScope;

        }

        public string Instance { get; }
        public string ClientId { get; }
        public string Domain { get; }
        public string SignedOutCallbackPath { get; }
        public string SignUpSignInPolicyId { get; }
        public string ClientSecret { get; }
        public string AccessTokenScope { get; }
    }
}
