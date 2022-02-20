using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Library.Serialisation
{
    public static class SerialisationSettings
    {
        public static JsonSerializerOptions DefaultOptions
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                options.Converters.Add(new JsonStringEnumConverter());

                return options;
            }
        }
    }
}
