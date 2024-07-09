using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;

namespace DertInfo.Api.ITests.Core
{
    public class IntergrationTestBase<O>
    {
        private const string bearerToken = "[PLACE A FUNCTIONAL BEARER TOKEN HERE TO RUN TESTS]";
        protected HttpClient _client;
        protected IHost _server;


        public IntergrationTestBase()
        {
        }

        public async Task InitialiseHelloWorldHostAndClient()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost => {
                    webHost.UseTestServer();
                    webHost.Configure(app => app.Run(async ctx =>
                        await ctx.Response.WriteAsync("Hello World!")));
                });

            // Build and start the IHost
            this._server = await hostBuilder.StartAsync();

            // Create an HttpClient to send requests to the TestServer
            this._client = this._server.GetTestClient();
        }

        public async Task InitialiseApplicationHostAndClient()
        {

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<DertInfo.Api.Startup>();
                });

            // Build and start the IHost
            this._server = await hostBuilder.StartAsync();

            // Create an HttpClient to send requests to the TestServer
            this._client = this._server.GetTestClient();
        }

        protected HttpRequestMessage ApplyAuthorisation(HttpRequestMessage request)
        {
            var token = string.Format("Bearer {0}", bearerToken);
            request.Headers.Add("Authorization", token);
            return request;
        }

        protected async Task<O> ReadResponseContent(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            O myObject = JsonConvert.DeserializeObject<O>(responseContent, GetSerialiserSettings());
            return myObject;
        }

        protected static JsonSerializerSettings GetSerialiserSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                Formatting = Formatting.None
            };
        }
    }
}
