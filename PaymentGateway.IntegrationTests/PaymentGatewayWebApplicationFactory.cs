using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api;
using PaymentGateway.Api.Data;
using PaymentGateway.Api.Interface;
using PaymentGateway.IntegrationTests.FakeServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.IntegrationTests
{
    public class PaymentGatewayWebApplicationFactory<T>
    : WebApplicationFactory<Startup> where T : class, IBankApi
    {
        private readonly IDictionary<string, string> _config = new Dictionary<string, string>
        {
            ["PaymentGatewayApi:Authentication:MetaDataAddress"] = "https://testing.com/.well-known/openid-configuration",
            ["PaymentGatewayApi:Authentication:ClientId"] = "CLIENT_ID",
            ["PaymentGatewayApi:Authentication:Audiences:0"] = "AUDIENCE_1",
            ["PaymentGatewayApi:Authentication:Audiences:1"] = "AUDIENCE_2",
            ["PaymentGatewayApi:Authentication:AuthoriseUri"] = "https://testing.com/oauth2/v2.0/authorize",
            ["PaymentGatewayApi:Authentication:TokenUri"] = "https://testing.com/oauth2/v2.0/token",
            ["PaymentGatewayApi:EventStoreConnectionString"] = "FAKE_EVENT_STORE",
            ["PaymentGatewayApi:SqlDbConnectionString"] = "FAKE_SQL_CONNECTION",
            ["PaymentGatewayApi:Bank:ApiKey"] = "FAKE_API_KEY",
            ["PaymentGatewayApi:Bank:BaseUrl"] = "https://www.fakebank.com/",
        };

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
              .ConfigureLogging(logging =>
              {
                  logging.ClearProviders();
                  logging.AddSerilog();
              })
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.ConfigureAppConfiguration(config =>
                  {
                      config.Sources.Clear();
                      config.AddInMemoryCollection(_config);
                  });
                  webBuilder.UseStartup<Startup>();
              }).UseSerilog();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var realDb = services.SingleOrDefault(
                    a=> a.ServiceType ==
                        typeof(DbContextOptions<PaymentApiDbContext>));

                var realApi = services.SingleOrDefault(
            a => a.ServiceType == typeof(IBankApi));


                var realEventStore = services.SingleOrDefault(
            a => a.ServiceType == typeof(IEventStore));

                services.Remove(realDb);
                services.Remove(realApi);
                services.Remove(realEventStore);

                services.AddSingleton<IEventStore, FakeEventStoreService>();
                services.AddSingleton<IBankApi, T>();

                services.AddDbContext<PaymentApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryDbForTesting-{Guid.NewGuid()}");
                }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            "Test", options => { });
            });
        }
    }
}
