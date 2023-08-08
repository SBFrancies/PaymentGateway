using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace PaymentGateway.BankSimulator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((ctx, builder) =>
            {
                var config = builder.Build();
                var keyVault = config["BankSimulator:KeyVault:BaseUrl"];

                if (!string.IsNullOrEmpty(keyVault))
                {
                    builder.AddAzureKeyVault(new Uri(keyVault), new DefaultAzureCredential());
                    config = builder.Build();
                }

                var tableStoreConnectionString = config["BankSimulator:TableStorage:ConnectionString"];

                if (!string.IsNullOrEmpty(tableStoreConnectionString))
                {
                    var tableStore = config["BankSimulator:TableStorage:TableName"];
                    Log.Logger = new LoggerConfiguration()
                   .Enrich.FromLogContext()
                   .WriteTo.Console()
                   .WriteTo.AzureTableStorage(tableStoreConnectionString, tableStore)
                   .CreateLogger();
                }
            })
            .UseSerilog();
    }
}
