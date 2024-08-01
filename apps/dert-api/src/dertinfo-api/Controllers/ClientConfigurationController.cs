using DertInfo.Models.DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{

    /// <summary>
    /// A public facing controller that allows retrival of information for the Dert of Derts submissions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientConfigurationController : Controller
    {
        private readonly IConfiguration _configuration;

        public ClientConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get the configurtion for the web client from Azure App Configuration where available.
        /// </summary>
        /// <returns></returns>
        [HttpGet("web")]
        public async Task<IActionResult> GetWebConfiguration()
        {
            var webClientSettings = new ClientConfigurationDto
            {
                AppInsightsTelemetryKey = _configuration["AppInsightsTelemetryKey"],
                Auth0Audience = _configuration["Auth0:Audience"],
                Auth0CallbackUrl = _configuration["WebClient:Auth0:CallbackUrl"], // http://localhost:44200
                Auth0ClientId = _configuration["WebClient:Auth0:ClientId"], 
                Auth0TenantDomain = _configuration["Auth0:Domain"]
            };

            return Ok(webClientSettings);
        }


        /// <summary>
        /// Get the configurtion for the mobile app from Azure App Configuration where available.
        /// </summary>
        /// <returns></returns>
        [HttpGet("app")]
        public async Task<IActionResult> GetAppConfiguration()
        {
            var webClientSettings = new ClientConfigurationDto
            {
                AppInsightsTelemetryKey = _configuration["AppInsightsTelemetryKey"],
                Auth0Audience = _configuration["Auth0:Audience"],
                Auth0CallbackUrl = _configuration["PwaClient:Auth0:CallbackUrl"], // http://localhost:44300
                Auth0ClientId = _configuration["PwaClient:Auth0:ClientId"],
                Auth0TenantDomain = _configuration["Auth0:Domain"]
            };



            return Ok(webClientSettings);
        }
    }
}
