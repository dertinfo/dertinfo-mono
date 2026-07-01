using Microsoft.ApplicationInsights;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Requests an access token from Auth0 to allow communication to the management api.
/// </summary>
namespace DertInfo.Services.ExternalProviders
{
    public interface IAuth0V2ManagementApiTokenRequester
    {
        Task<string> GetManagementApiToken(string authDomain, string authClientId, string authClientSecret);
    }

    public class Auth0V2ManagementApiTokenRequester : IAuth0V2ManagementApiTokenRequester
    {
        private readonly TelemetryClient _telemetryClient;

        public Auth0V2ManagementApiTokenRequester(TelemetryClient telemetryClient = null)
        {
            this._telemetryClient = telemetryClient;
        }

        public async Task<string> GetManagementApiToken(string authDomain, string authClientId, string authClientSecret)
        {
            // If application insights is running then log this call.
            if (this._telemetryClient != null)
            {
                // Just get enough inforamtion from the secret to be able to diagnose issues.
                var managementClientSecretObfuscated = $"{authClientSecret.Substring(0, 4)}...{authClientSecret.Substring(authClientSecret.Length - 4, 4)}";

                IDictionary<string, string> telemetryProperties = new Dictionary<string, string>();
                telemetryProperties["Auth0:Domain"] = authDomain;
                telemetryProperties["Auth0:ManagementClientId"] = authClientId;
                telemetryProperties["Auth0:ManagementClientSecret"] = managementClientSecretObfuscated;

                this._telemetryClient.TrackTrace("External Call To Auth0 for Token:", telemetryProperties);
            }

            var baseEndpoint = "https://" + authDomain + "/";
            var managementEndpoint = "https://" + authDomain + "/api/v2/";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseEndpoint);

            StringContent bodyContent = new StringContent("{\"grant_type\":\"client_credentials\",\"client_id\": \"" + authClientId + "\",\"client_secret\": \"" + authClientSecret + "\",\"audience\": \"" + managementEndpoint + "\"}", Encoding.UTF8, "application/json");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync("oauth/token", bodyContent);
            if (response.IsSuccessStatusCode)
            {
                var access_token = string.Empty;
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    JObject responseJObject = JObject.Parse(responseContent);
                    foreach (KeyValuePair<String, JToken> kvp in responseJObject)
                    {
                        if (kvp.Key == "access_token") { access_token = kvp.Value.ToString(); }
                    }
                }

                return access_token;
            }
            else
            {
                throw new HttpRequestException("API to Auth0 Management Api connection failed. ResponseCode:" + response.StatusCode);
            }
        }
    }
}
