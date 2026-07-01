using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.SystemSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DodSettingAuthController : AuthController
    {
        private const string _openToPublicKey = "SYSTEM-DOD-OPENFORPUBLIC";
        private const string _resultsPublishedKey = "SYSTEM-DOD-RESULTSPUBLISHED";
        private const string _publicResultsForwarded = "SYSTEM-DOD-PUBLICRESULTSFORWARDED";
        private const string _officialResultsForwarded = "SYSTEM-DOD-OFFICIALRESULTSFORWARDED";
        private const string _judgePasswordsKey = "SYSTEM-DOD-JUDGEPASSWORDS";

        private ISystemSettingService _systemSettingService;

        public DodSettingAuthController(
            ISystemSettingService systemSettingService,
            IDertInfoUser user
            ) : base(user)
        {
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetSettings()
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var allSettings = await _systemSettingService.ListAll();

            DertOfDertSettingsDto dertOfDertsSettingsDto = new DertOfDertSettingsDto()
            {
                OpenToPublic = bool.Parse(allSettings.First(s => s.Ref == _openToPublicKey).Value),
                ResultsPublished = bool.Parse(allSettings.First(s => s.Ref == _resultsPublishedKey).Value),
                PublicResultsForwarded = bool.Parse(allSettings.First(s => s.Ref == _publicResultsForwarded).Value),
                OfficialResultsForwarded = bool.Parse(allSettings.First(s => s.Ref == _officialResultsForwarded).Value),
                ValidJudgePasswords = allSettings.First(s => s.Ref == _judgePasswordsKey).Value
            };

            return Ok(dertOfDertsSettingsDto);
        }

        [HttpPut]
        [Route("opentopublic")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> SetOpenToPublic([FromBody] DertOfDertSettingsUpdateSubmissionDto settingUpdate)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _systemSettingService.UpdateSettingByKeyAndValue(_openToPublicKey, settingUpdate.BooleanValue.ToString());

            return Ok(true);
        }

        [HttpPut]
        [Route("publicresultsforwarded")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> SetPublicResultsForwarded([FromBody] DertOfDertSettingsUpdateSubmissionDto settingUpdate)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _systemSettingService.UpdateSettingByKeyAndValue(_publicResultsForwarded, settingUpdate.BooleanValue.ToString());

            return Ok(true);
        }

        [HttpPut]
        [Route("officialresultsforwarded")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> SetOfficialResultsForwarded([FromBody] DertOfDertSettingsUpdateSubmissionDto settingUpdate)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _systemSettingService.UpdateSettingByKeyAndValue(_officialResultsForwarded, settingUpdate.BooleanValue.ToString());

            return Ok(true);
        }

        [HttpPut]
        [Route("resultspublished")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> SetResultsPublished([FromBody] DertOfDertSettingsUpdateSubmissionDto settingUpdate)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _systemSettingService.UpdateSettingByKeyAndValue(_resultsPublishedKey, settingUpdate.BooleanValue.ToString());

            return Ok(true);
        }

        [HttpPut]
        [Route("validjudgepasswords")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> SetValidJudgePasswords([FromBody] DertOfDertSettingsUpdateSubmissionDto settingUpdate)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _systemSettingService.UpdateSettingByKeyAndValue(_judgePasswordsKey, settingUpdate.StringValue);

            return Ok(true);
        }


    }
}
