using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PaymentGateway.Merchant.Apis;
using PaymentGateway.Merchant.Interface;
using PaymentGateway.Merchant.Models.Settings;

namespace PaymentGateway.Merchant
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
            services.AddDistributedMemoryCache();

            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, $"Merchant:{Constants.AzureAdB2C}")
                    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "https://PaymentGatewayAD.onmicrosoft.com/f5fd2651-c11f-490b-9e81-f3933aa7a0ac/Payments.ReadWrite" })
                    .AddDistributedTokenCaches();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.CheckConsentNeeded = context => true;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                              ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            var appsettings = new MerchantAppSettings();
            Configuration.Bind("Merchant", appsettings);
            var settings = new MerchantSettings(appsettings);

            services.AddSingleton(settings);
            services.AddHttpClient("PaymentGateway", a =>
            {
                a.BaseAddress = settings.PaymentGateway.BaseUrl;
            });

            services.AddRazorPages();
            services.AddScoped<IGatewayApi, PaymentGatewayApi>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseForwardedHeaders();

            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.None,
                CheckConsentNeeded = context => true,
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use((context, next) =>
            {
                try
                {
                    return next(context);
                }

                catch (MicrosoftIdentityWebChallengeUserException)
                {
                    context.Response.Redirect("/");
                    return next(context);
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
