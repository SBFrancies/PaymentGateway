using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Library.Models.Settings
{
    public class TableStorageAppSettings
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }
    }
}
