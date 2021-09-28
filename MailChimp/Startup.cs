using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MailChimp.Net.Interfaces;
using System;
using MailChimp.Net;
using Mailchimp.Services;
using System.Net.Http;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Net.Http.Headers;

[assembly: FunctionsStartup(typeof(MailChimp.Startup))]

namespace MailChimp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();            

            builder.Services.AddScoped<IMailChimpManager>(m =>
            {
                var apiKey = Environment.GetEnvironmentVariable("ApiKey");
                return new MailChimpManager(apiKey);
            });

            builder.Services.AddScoped<RegistrationService>(o =>
            {
                var clientId = Environment.GetEnvironmentVariable("ClientId");
                var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
                var authority = string.Concat(Environment.GetEnvironmentVariable("Instance"), Environment.GetEnvironmentVariable("TenantId"));

                var aadClient = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(authority)
                    .Build();

                string[] scopes = new string[] { Environment.GetEnvironmentVariable("Scope") };
                var authResult = aadClient.AcquireTokenForClient(scopes).ExecuteAsync().Result;

                var httpClient = o.GetRequiredService<HttpClient>();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                var apiUrl = Environment.GetEnvironmentVariable("RegistrationApi");
                return new RegistrationService(apiUrl, httpClient);
            });
        }
    }
}
