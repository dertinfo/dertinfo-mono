using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.CrossCutting.Configuration
{
    public interface IDertInfoConfiguration
    {
        string Auth0_Domain { get; }
        string Auth0_ManagementClientId { get; }
        string Auth0_ManagementClientSecret { get; }
        string SendGrid_ApiKey { get; }
        bool SendGrid_Enabled { get; }
        string Defaults_GroupImageName { get; }
        string Defaults_EventImageName { get; }
        int Constants_ObfuscationIdLength { get; }
        bool DatabaseCache_Disabled { get; }
    }
    public class DertInfoConfiguration: IDertInfoConfiguration
    {

        private IConfiguration _configuration;
        public DertInfoConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Auth0_Domain { get { return this._configuration["Auth0:Domain"]; } }
        public string Auth0_ManagementClientId { get { return this._configuration["Auth0:ManagementClientId"]; } }
        public string Auth0_ManagementClientSecret { get { return this._configuration["Auth0:ManagementClientSecret"]; } }
        public string SendGrid_ApiKey { get { return this._configuration["SendGrid:ApiKey"]; } }
        public bool SendGrid_Enabled { get { return bool.Parse(this._configuration["SendGrid:Enabled"]); } }
        public string Defaults_GroupImageName { get { return this._configuration["Constants:Defaults:GroupImageName"]; } }
        public string Defaults_EventImageName { get { return this._configuration["Constants:Defaults:EventImageName"]; } }
        public string AzureStorageAccount_TeamPicturesStorageContainer { get { return this._configuration["AzureStorageAccount:TeamPicturesStorageContainer"]; } }
        public string AzureStorageAccount_EventPicturesStorageContainer { get { return this._configuration["AzureStorageAccount:EventPicturesStorageContainer"]; } }
        public int Constants_ObfuscationIdLength { get { return int.Parse(this._configuration["Constants:ObfuscationIdLength"]); } }
        public bool DatabaseCache_Disabled { get { return bool.Parse(this._configuration["DatabaseCache:Disabled"]); } }
    }
}
