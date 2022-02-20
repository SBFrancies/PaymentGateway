using PaymentGateway.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Library.Models.Settings
{
    public class TableStorageSettings
    {
        public TableStorageSettings(TableStorageAppSettings appsettings)
        {
            if (appsettings == null)
            {
                throw new ArgumentNullException(nameof(appsettings));
            }

            if (string.IsNullOrEmpty(appsettings.ConnectionString) && !string.IsNullOrEmpty(appsettings.TableName))
            {
                throw new InvalidConfigurationException(nameof(appsettings.ConnectionString), "ConnectionString can be populated only if TableName is populated");
            }

            if (!string.IsNullOrEmpty(appsettings.ConnectionString) && string.IsNullOrEmpty(appsettings.TableName))
            {
                throw new InvalidConfigurationException(nameof(appsettings.TableName), "TableName can be populated only if ConnectionString is populated");
            }

            ConnectionString = appsettings.ConnectionString;
            TableName = appsettings.TableName;
        }

        public string ConnectionString { get;  }

        public string TableName { get;  }
    }
}
