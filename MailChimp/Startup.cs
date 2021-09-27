using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MailChimp.Net.Interfaces;
using System;
using MailChimp.Net;

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
        }
    }
}
