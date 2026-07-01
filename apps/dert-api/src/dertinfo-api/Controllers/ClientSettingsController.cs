using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodSubmissions;
using DertInfo.Services.Entity.DodUsers;
using DertInfo.Services.Entity.SystemSettings;
using Microsoft.AspNetCore.Mvc;

namespace DertInfo.Api.Controllers
{

    /// <summary>
    /// A public facing controller that allows retrival of information for the Dert of Derts submissions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSettingsController : Controller
    {
        private string _errorMessage = string.Empty;
        private const string _dodOpenToPublicKey = "SYSTEM-DOD-OPENFORPUBLIC";
        private const string _dodResultsPublishedKey = "SYSTEM-DOD-RESULTSPUBLISHED";
        private const string _dodPublicResultsForwardedKey = "SYSTEM-DOD-PUBLICRESULTSFORWARDED";
        private const string _dodOfficialResultsForwardedKey = "SYSTEM-DOD-OFFICIALRESULTSFORWARDED";

        ISystemSettingService _systemSettingService;


        public ClientSettingsController(
            ISystemSettingService systemSettingService
            )
        {
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var systemSettings = await _systemSettingService.GetByKeys(new List<string>() { _dodOpenToPublicKey, _dodResultsPublishedKey, _dodPublicResultsForwardedKey, _dodOfficialResultsForwardedKey });

            var clientSettings = new ClientSettingsDto();
            clientSettings.DodOpenToPublic = bool.Parse(systemSettings.First(ss => ss.Ref == _dodOpenToPublicKey).Value);
            clientSettings.DodResultsPublished = bool.Parse(systemSettings.First(ss => ss.Ref == _dodResultsPublishedKey).Value);
            clientSettings.DodPublicResultsForwarded = bool.Parse(systemSettings.First(ss => ss.Ref == _dodPublicResultsForwardedKey).Value);
            clientSettings.DodOfficialResultsForwarded = bool.Parse(systemSettings.First(ss => ss.Ref == _dodOfficialResultsForwardedKey).Value);

            return Ok(clientSettings);
        }
    }
}
