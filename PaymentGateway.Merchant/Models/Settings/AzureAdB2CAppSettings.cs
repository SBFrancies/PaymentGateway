namespace PaymentGateway.Merchant.Models.Settings
{
    public class AzureAdB2CAppSettings
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string Domain { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessTokenScope { get; set; }
    }
}
