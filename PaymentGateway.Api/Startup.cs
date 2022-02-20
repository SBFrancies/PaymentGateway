using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentGateway.Api.Data;
using PaymentGateway.Api.Data.Entities;
using PaymentGateway.Api.DataAccess;
using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Jobs;
using PaymentGateway.Api.Mapping;
using PaymentGateway.Api.Models.BankApi;
using PaymentGateway.Api.Models.Request;
using PaymentGateway.Api.Models.Response;
using PaymentGateway.Api.Models.Settings;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validation;
using PaymentGateway.Library.Interface;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PaymentGateway.Api
{
    public class Startup
    {
        private const string ApiTitle = "Payment Gateway API";
        private PaymentGatewaySettings Settings { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(a =>
                {
                    a.MetadataAddress = Settings.Authentication.MetaDataAddress;
                    a.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = Settings.Authentication.Audiences,
                        NameClaimType = "name",
                    };
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
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = Settings.Authentication.AuthoriseUri,
                            TokenUrl = Settings.Authentication.TokenUri,
                            Scopes = new Dictionary<string, string>
                             {
                                { "openid", "openid" },
                                { "offline_access", "offline_access" },
                                {Settings.Authentication.ClientId, Settings.Authentication.ClientId }
                             },
                        }
                    }
                };

                options.AddSecurityDefinition("oauth2", definition);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },

                    }] = new string[] { },
                });
            });

            var appsettings = new PaymentGatewayAppSettings();
            Configuration.Bind("PaymentGatewayApi", appsettings);
            Settings = new PaymentGatewaySettings(appsettings);

            services.AddSingleton(Settings);
            services.AddHttpClient("BankApi", a =>
            {
                a.BaseAddress = Settings.Bank.BaseUrl;
            });
            services.AddSingleton<IPaymentService, PaymentService>();
            services.AddSingleton<IBankApi, BankApiService>();
            services.AddSingleton<ICardNumberHider, CardNumberHider>();
            services.AddSingleton<IIdentifierGenerator, IdentifierGenerator>();
            services.AddSingleton<IEventStore, EventStoreService>();
            services.AddSingleton<IPaymentDataAccess, PaymentDataAccess>();
            services.AddSingleton<IBankStatusHolder, BankApiStatusService>();
            services.AddSingleton<IValidator<CreatePaymentRequest>, PaymentValidator>();
            services.AddSingleton<IMapper<CreatePaymentRequest, CardPaymentRequest>, CreatePaymentToBankApiPaymentRequestMapper>();
            services.AddSingleton<IMapper<CreatePaymentRequest, PaymentTable>, CreatePaymentToPaymentTableMapper>();
            services.AddSingleton<IMapper<PaymentTable, PaymentDetails>, PaymentTableToPaymentDetailsMapper>();
            services.AddHttpContextAccessor();
            services.AddHostedService<BankHealthCheckJob>();

            services.AddDbContext<PaymentApiDbContext>(a =>
                {
                    a.UseSqlServer(Settings.SqlDbConnectionString, b => b.EnableRetryOnFailure());

                }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

            services.AddSingleton<Func<PaymentApiDbContext>>(a => () => a.GetRequiredService<PaymentApiDbContext>());
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiTitle);
                c.DocumentTitle = ApiTitle;
                c.EnableDeepLinking();
                c.OAuthClientId(Settings.Authentication.ClientId);
                c.OAuthUsePkce();
                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
