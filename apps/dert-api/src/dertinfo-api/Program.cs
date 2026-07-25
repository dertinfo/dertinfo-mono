using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using System;

namespace DertInfo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// ASP.NET Core generic host.
        /// Hosted: when AZURE_APP_CONFIG is set, Azure App Configuration (+ Key Vault) overlays appsettings.json.
        /// Local: CreateDefaultBuilder already applies process environment variables (e.g. from infra/secrets/api.env
        /// injected by npm run start or Compose env_file) with standard configuration precedence.
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                LoadConfiguration(config);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        private static void LoadConfiguration(IConfigurationBuilder config)
        {
            var appConfigurationUri = Environment.GetEnvironmentVariable("AZURE_APP_CONFIG");

            if (appConfigurationUri != null)
            {
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
                // note - In package version 1.3 of the Azure.Identity there is a bug with the Shared Token Cache Credential and therefore we omit it.

                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(appConfigurationUri), credential)
                    .Select(KeyFilter.Any, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(credential);
                    });
                });
            }
        }
    }
}
