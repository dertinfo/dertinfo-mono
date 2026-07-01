using Azure.Identity;
using Microsoft.AspNetCore.Builder;
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
        /// This is the new host builder as of ASP.NET Core 3.1. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>Formally this used WebHostBuilder which is still available (09/11/2020) but will be deprecated soon.</remarks>
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

        /// <summary>
        /// When in production or test we want to use the configuration in the AzureAppConfiruation && AzurKeyVault.
        /// In development we want to use the "Secrets Manager Tool" which will get the local secrets at "%APP_DATA%\Microsoft\UserSecrets\<app_secrets_id>\secrets.json.
        /// note - In order for this to be the most secure we need to isolate the external services or not use them. e.g SendGrid (Auth0 is isolated)
        /// note - secrets are no longer in appsettings.json but those connections to live services should not be exposed to other devs.      
        /// </summary>
        /// <param name="config"></param>
        private static void LoadConfiguration(IConfigurationBuilder config)
        {
            // Identify and collect the App Configuration Information if set in the envionment variable.
            var appConfigurationUri = Environment.GetEnvironmentVariable("AZURE_APP_CONFIG");

            // If we have an app configuration variable. Then configure it (All envionments but development which uses secrets).
            if (appConfigurationUri != null)
            {
                // Build the Config
                var settings = config.Build();

                // Get the access credentials from the runtime envionment
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });
                // note - In package version 1.3 of the Azure.Identity there is a bug with the Shared Token Cache Credential and therefore we omit it.

                // Add app configuration with key vault support
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(appConfigurationUri), credential)
                    // Collect any App Configuration Settings with the Envionment Label
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
