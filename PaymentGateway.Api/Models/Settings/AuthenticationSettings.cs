using PaymentGateway.Library.Exceptions;
using System;
using System.Linq;

namespace PaymentGateway.Api.Models.Settings
{
    public class AuthenticationSettings
    {
        public AuthenticationSettings(AuthenticationAppSettings appsettings)
        {
            if(appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if(string.IsNullOrEmpty(appsettings.MetaDataAddress))
            {
                throw new InvalidConfigurationException(nameof(appsettings.MetaDataAddress), "MetaDataAddress cannot be null or empty");
            }

            if (string.IsNullOrEmpty(appsettings.ClientId))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ClientId), "ClientId cannot be null or empty");
            }

            if(appsettings.Audiences == null || !appsettings.Audiences.Any())
            {
                throw new InvalidConfigurationException(nameof(appsettings.Audiences), "At least one valid audience must be specified");
            }

            if(!Uri.IsWellFormedUriString(appsettings.AuthoriseUri, UriKind.Absolute))
            {
                throw new InvalidConfigurationException(nameof(appsettings.AuthoriseUri), "AuthoriseUri must be a valid absolute URI");
            }

            if (!Uri.IsWellFormedUriString(appsettings.TokenUri, UriKind.Absolute))
            {
                throw new InvalidConfigurationException(nameof(appsettings.TokenUri), "TokenUri must be a valid absolute URI");
            }

            MetaDataAddress = appsettings.MetaDataAddress;
            ClientId = appsettings.ClientId;    
            Audiences = appsettings.Audiences;
            AuthoriseUri = new Uri(appsettings.AuthoriseUri);
            TokenUri = new Uri(appsettings.TokenUri);
        }

        public string MetaDataAddress { get;  }

        public string ClientId { get;  }

        public string[] Audiences { get;  }

        public Uri AuthoriseUri { get; }

        public Uri TokenUri { get; }
    }
}

