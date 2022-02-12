using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(a =>
                {
                    a.MetadataAddress = "https://paymentgatewayad.b2clogin.com/PaymentGatewayAD.onmicrosoft.com/B2C_1_PaymentGateway/v2.0/.well-known/openid-configuration";
                    a.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = new[] { "263ca8c6-28a0-4d99-bf69-d4d198da3692", "f5fd2651-c11f-490b-9e81-f3933aa7a0ac" }
                    };
                });
            services.AddControllers();

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
                            AuthorizationUrl = new Uri("https://paymentgatewayad.b2clogin.com/paymentgatewayad.onmicrosoft.com/b2c_1_paymentgateway/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri("https://paymentgatewayad.b2clogin.com/paymentgatewayad.onmicrosoft.com/b2c_1_paymentgateway/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                             {
                                { "openid", "openid" },
                                { "offline_access", "offline_access" },
                                {"263ca8c6-28a0-4d99-bf69-d4d198da3692","263ca8c6-28a0-4d99-bf69-d4d198da3692" }
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API");
                c.DocumentTitle = "Payment Gateway API";
                c.EnableDeepLinking();
                c.OAuthClientId("263ca8c6-28a0-4d99-bf69-d4d198da3692");
                c.OAuthUsePkce();
                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
