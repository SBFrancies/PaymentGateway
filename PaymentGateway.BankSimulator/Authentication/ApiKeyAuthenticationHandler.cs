using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGateway.BankSimulator.Interface;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeader = "X-API-KEY";

        private IClientRepository ClientRepository { get; set; }

        public ApiKeyAuthenticationHandler(IClientRepository clientRepository, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            ClientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeader, out var header))
            {
                return AuthenticateResult.Fail("Authorization header missing");
            }

            string key;

            try
            {
                var authorizationHeader = header.ToString();
                var bytes = Convert.FromBase64String(authorizationHeader);
                key = Encoding.UTF8.GetString(bytes);
            }

            catch (Exception exception)
            {
                Logger.LogError(exception, "Error processing Authorization header");
                return AuthenticateResult.Fail("Invalid Authorization header");
            }

            var client = await ClientRepository.GetClientAsync(key);

            if (client == null)
            {
                return AuthenticateResult.Fail("Invalid API key detected");
            }

            var authenticatedUser = new User("ApiKeyAuthentication", true, client.ApiKey);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}