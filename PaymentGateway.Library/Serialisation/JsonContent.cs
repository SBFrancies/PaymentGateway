using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PaymentGateway.Library.Serialisation
{
    public class JsonContent : StringContent
    {
        public JsonContent(object content) : base(JsonSerializer.Serialize(content, SerialisationSettings.DefaultOptions), Encoding.UTF8, "application/json")
        {
        }
    }
}
