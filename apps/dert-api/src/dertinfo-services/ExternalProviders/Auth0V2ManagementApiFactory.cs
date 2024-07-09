using Auth0.ManagementApi;
using DertInfo.CrossCutting.Configuration;
using EnsureThat;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.ExternalProviders
{
    public interface IAuth0V2ManagementApiFactory
    {
        Task<IAuth0V2ManagementApiFacade> GetClient();
    }

    public class Auth0V2ManagementApiFactory : IAuth0V2ManagementApiFactory
    {
        private readonly string _authClientId;
        private readonly string _authClientSecret;
        private readonly string _authDomain;
        private readonly IAuth0V2ManagementApiTokenRequester _tokenRequester;
        private string _authToken;

        public Auth0V2ManagementApiFactory(IDertInfoConfiguration configuration, IAuth0V2ManagementApiTokenRequester tokenRequester)
        {
            this._authClientId = configuration.Auth0_ManagementClientId;
            this._authClientSecret = configuration.Auth0_ManagementClientSecret;
            this._authDomain = configuration.Auth0_Domain;
            this._tokenRequester = tokenRequester;
        }

        public async Task<IAuth0V2ManagementApiFacade> GetClient()
        {
            Ensure.String.IsNotNullOrEmpty(this._authDomain, nameof(this._authDomain));
            Ensure.String.IsNotNullOrEmpty(this._authClientId, nameof(this._authDomain));
            Ensure.String.IsNotNullOrEmpty(this._authClientSecret, nameof(this._authDomain));

            var authManagementApiEndpoint = "https://" + this._authDomain + "/api/v2";

            if (this._authToken == null || this._authToken == string.Empty)
            {
                this._authToken = await _tokenRequester.GetManagementApiToken(this._authDomain, this._authClientId, this._authClientSecret);
            }

            if (this._authDomain != string.Empty && this._authToken != string.Empty)
            {
                var facade = new Auth0V2ManagementApiFacade();
                facade.Connect(this._authToken, new Uri(authManagementApiEndpoint));
                return facade;
            }
            else
            {
                throw new ArgumentException("Cannot connect to auth0 management Api if the endpoint and credentials are not provided");
            }
        }
    }
}
