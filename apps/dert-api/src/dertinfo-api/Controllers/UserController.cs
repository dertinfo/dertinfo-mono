using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DertInfo.CrossCutting.Connection;
using DertInfo.Services;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.System;
using AutoMapper;
using DertInfo.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IAuthService _authService;
        IDataObfuscationService _dataObfuscationService;
        IUserService _userService;

        public UserController(
            IMapper mapper,
            IAuthService authService,
            IDertInfoUser user,
            IDataObfuscationService dataObfuscationService,
            IUserService userService
            ) : base(user)
        {
            _mapper = mapper;
            _authService = authService;
            _dataObfuscationService = dataObfuscationService;
            _userService = userService;
        }

        [HttpGet]
        [Route("overview", Name = "GetUserOverview")]
        public async Task<IActionResult> GetUserOverview()
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var userOverview = await this._userService.GetOverview();

            UserOverviewDto userOverviewDto = _mapper.Map<UserOverviewDto>(userOverview);

            return Ok(userOverviewDto);
        }

        [HttpPost]
        [Route("gdprinformation", Name = "AddGdprInformationToUser")]
        public async Task<IActionResult> AddGdprInformationToUser([FromBody]UserGdprInformationSubmissionDto userGdprInformationSubmissionDto)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            UserGdprInformation userGdprInformation = new UserGdprInformation();
            userGdprInformation.GdprConsentGained = userGdprInformationSubmissionDto.GdprConsentGained;
            userGdprInformation.Auth0UserId = _user.AuthId;

            userGdprInformation = await _authService.AddGdprInformation(userGdprInformation);

            UserGdprInformationDto userGdprInformationDto = _mapper.Map<UserGdprInformationDto>(userGdprInformation);

            return Created("", userGdprInformationDto);
        }

        [HttpPut]
        [Route("settings", Name = "UpdateUserSettings")]
        public async Task<IActionResult> UpdateUserSettings([FromBody]UserSettingsUpdateSubmissionDto userSettingsUpdateSubmissionDto)
        {
            if (!ValidUserSettingsUpdate(userSettingsUpdateSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            UserSettings myUserSettings = new UserSettings();
            myUserSettings.FirstName = userSettingsUpdateSubmissionDto.FirstName;
            myUserSettings.LastName = userSettingsUpdateSubmissionDto.LastName;
            myUserSettings.Telephone = userSettingsUpdateSubmissionDto.Telephone;

            myUserSettings = await this._userService.UpdateCurrentUserSettings(myUserSettings);

            return NoContent();
        }

        [HttpDelete]
        [Route("deleteaccount", Name = "DeleteAccount")]
        public async Task<IActionResult> RemoveAccount()
        {
            this.ExtractUser();

            // todo - there needs to be a check here to ensure that they are the only admin. - policy tbd for handling this. 
            //      - at current there is typically only 1 admin per group.

            foreach (var strGroupId in _user.ClaimsGroupAdmin)
            {
                await _dataObfuscationService.ObfuscateGroupInformation(int.Parse(strGroupId));
            }

            await _authService.DeleteUserAccount(_user.AuthId);

            return Ok();
        }

        #region Submission Validation
        private bool ValidUserSettingsUpdate(UserSettingsUpdateSubmissionDto userSettingsUpdateSubmissionDto)
        {
            if (userSettingsUpdateSubmissionDto == null)
            {
                this._errorMessage = "userSettings submission is null";
                return false;
            }

            if (userSettingsUpdateSubmissionDto.FirstName == string.Empty)
            {
                this._errorMessage = "firstname must be supplied";
                return false;
            }

            if (userSettingsUpdateSubmissionDto.LastName == string.Empty)
            {
                this._errorMessage = "lastname must be supplied";
                return false;
            }

            return true;
        }

        #endregion
    }
}
