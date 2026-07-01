using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Services.Entity.SystemSettings;
using DertInfo.Models.DataTransferObject;
using AutoMapper;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SystemSettingController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        private ISystemSettingService _systemSettingService;

        public SystemSettingController(
            IMapper mapper,
            IDertInfoUser user,
            ISystemSettingService systemSettingService
            ) : base(user) {
            _mapper = mapper;
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdministratorOnlyPolicy")]
        public async Task<IActionResult> Get()
        {
            // Validate

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var systemSettings = await _systemSettingService.ListAll();

            // Perform Auto Map of simple fields
            List<SystemSettingDto> systemSettingDtos = _mapper.Map<List<SystemSettingDto>>(systemSettings);

            return Ok(systemSettingDtos);

        }

        [HttpPut]
        [Authorize(Policy = "SuperAdministratorOnlyPolicy")]
        public async Task<IActionResult> Put([FromBody] SystemSettingUpdateSubmissionDto systemSettingUpdateSubmissionDto )
        {
            // Validate
            if (!ValidSystemSettingUpdateSubmission(systemSettingUpdateSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var systemSetting = await _systemSettingService.UpdateSettingByKeyAndValue(systemSettingUpdateSubmissionDto.Key, systemSettingUpdateSubmissionDto.Value);

            return Ok(systemSetting);

        }

        private bool ValidSystemSettingUpdateSubmission(SystemSettingUpdateSubmissionDto systemSettingUpdateSubmissionDto)
        {
            if (systemSettingUpdateSubmissionDto == null)
            {
                this._errorMessage = "system setting submission is null";
                return false;
            }

            if (systemSettingUpdateSubmissionDto.Key == string.Empty)
            {
                this._errorMessage = "key must be supplied";
                return false;
            }

            if (systemSettingUpdateSubmissionDto.Value == null)
            {
                this._errorMessage = "value must be supplied";
                return false;
            }

            return true;
        }
    }
}
