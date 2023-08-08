using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.BankSimulator.Authentication;
using PaymentGateway.BankSimulator.Interface;
using PaymentGateway.BankSimulator.Models.Settings;
using PaymentGateway.BankSimulator.Service;
using PaymentGateway.BankSimulator.Service.Fakes;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.BankSimulator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("ApiKeyAuthentication")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyAuthentication", null);

            services.AddAuthorization(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.AddPolicy("ApiKey", policy);
            });

            services.AddControllers().AddJsonOptions(a =>
            {
                a.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSwaggerGen(options =>
            {
                var definition = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "apikey",
                    Name = "X-API-KEY",
                    In = ParameterLocation.Header,
                };

                options.AddSecurityDefinition("apikey", definition);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "apikey"
                        },

                    }] = new string[] { },
                });
            });

            services.AddHealthChecks();

            var appsettings = new BankSimulatorAppSettings();
            Configuration.Bind("BankSimulator", appsettings);
            var settings = new BankSimulatorSettings(appsettings);

            services.AddSingleton(settings);
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IBankCodeGenerator, BankCodeGenerator>();
            services.AddSingleton<IClientRepository, FakeClientRepository>();
            services.AddSingleton<IPaymentProcessor, FakePaymentProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Simulator API");
                c.DocumentTitle = "Bank Simulator API";
                c.EnableDeepLinking();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        [HealthStatus.Healthy] = (int)HttpStatusCode.OK,
                        [HealthStatus.Degraded] = (int)HttpStatusCode.OK,
                        [HealthStatus.Unhealthy] = (int)HttpStatusCode.ServiceUnavailable,
                    },
                }).AllowAnonymous();
            });
        }
    }
}
