using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Api.Data;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.IntegrationTests
{
    public class PaymentGatewayWebApplicationFactory
    : WebApplicationFactory<Startup>
    {
        private readonly IDictionary<string, string> _config = new Dictionary<string, string>
        {
            ["PaymentGatewayApi:Authentication:MetaDataAddress"] = "https://testing/.well-known/openid-configuration",
            ["PaymentGatewayApi:Authentication:ClientId"] = "CLIENT_ID",
            ["PaymentGatewayApi:Authentication:Audiences:0"] = "AUDIENCE_1",
            ["PaymentGatewayApi:Authentication:Audiences:1"] = "AUDIENCE_2",
            ["PaymentGatewayApi:Authentication:AuthoriseUri"] = "https://testing/oauth2/v2.0/authorize",
            ["PaymentGatewayApi:Authentication:TokenUri"] = "https://testing/oauth2/v2.0/token",
        };

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(webBuilder => {
                webBuilder.Sources.Clear();
                webBuilder.AddInMemoryCollection(_config);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<PaymentApiDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<PaymentApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            "Test", options => { });
            });
        }
    }
}
