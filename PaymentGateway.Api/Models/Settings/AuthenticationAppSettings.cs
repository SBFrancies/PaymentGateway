namespace PaymentGateway.Api.Models.Settings
{
    public class AuthenticationAppSettings
    {
        public string MetaDataAddress { get; set; }

        public string ClientId { get; set; }

        public string[] Audiences { get; set; }

        public string AuthoriseUri { get; set; }

        public string TokenUri { get; set; }
    }
}
