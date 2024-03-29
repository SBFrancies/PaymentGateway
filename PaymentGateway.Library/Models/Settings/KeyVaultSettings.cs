﻿using System;

namespace PaymentGateway.Library.Models.Settings
{
    public class KeyVaultSettings
    {
        public KeyVaultSettings(KeyVaultAppSettings appsettings)
        {
            if (appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            BaseUrl = appsettings.BaseUrl;
        }

        public string BaseUrl { get;}
    }
}
