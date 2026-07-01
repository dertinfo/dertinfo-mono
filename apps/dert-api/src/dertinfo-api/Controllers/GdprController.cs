using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GdprController : AuthController
    {
        private IDataObfuscationService _dataObfuscationService;

        public GdprController(
            IDertInfoUser user,
            IDataObfuscationService dataObfuscationService
            ) : base(user) {
            _dataObfuscationService = dataObfuscationService;
        }

        [HttpPost]
        [Route("group/{groupId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> ObfuscateGroupInformation([FromRoute] int groupId)
        {
            // Validate

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            await _dataObfuscationService.ObfuscateGroupInformation(groupId);

            return Ok();
            
        }
    }
}
