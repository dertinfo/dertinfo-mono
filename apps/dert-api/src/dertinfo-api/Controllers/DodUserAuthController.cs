using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodUsers;
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
    public class DodUserAuthController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodResultService _dodResultService;
        IDodUserService _dodUserService;

        public DodUserAuthController(
            IMapper mapper,
            IDodResultService dodResultService,
            IDodUserService dodUserService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _dodResultService = dodResultService;
            _dodUserService = dodUserService;
        }

        [HttpGet]
        [Route("judges")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetJudgesInfo()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var judges = await _dodUserService.ListJudgesWithCompletionCounts();

            List<DodJudgeInfoDto> dodJudgeInfoDto = _mapper.Map<List<DodJudgeInfoDto>>(judges);

            return Ok(dodJudgeInfoDto.OrderBy(dto => dto.JudgeName));
        }

        [HttpPut]
        [Route("blockuser")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> BlockUser(DodBlockUserSubmissionDto blockUserSubmissionDto)
        {
            if (!ValidBlockUserSubmission(blockUserSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            if (blockUserSubmissionDto.Block)
            {
                return Ok(await _dodUserService.BlockUser(blockUserSubmissionDto.DodUserId));
            }

            if (blockUserSubmissionDto.UnBlock)
            {
                return Ok(await _dodUserService.UnBlockUser(blockUserSubmissionDto.DodUserId));
            }

            return BadRequest();
        }

        private bool ValidBlockUserSubmission(DodBlockUserSubmissionDto blockUserSubmissionDto)
        {
            if (blockUserSubmissionDto == null)
            {
                this._errorMessage = "block user submission is null";
                return false;
            }

            if (blockUserSubmissionDto.DodUserId == 0)
            {
                this._errorMessage = "userId must be supplied";
                return false;
            }

            if (!blockUserSubmissionDto.Block && !blockUserSubmissionDto.UnBlock)
            {
                this._errorMessage = "no action to take";
                return false;
            }

            if (blockUserSubmissionDto.Block && blockUserSubmissionDto.UnBlock)
            {
                this._errorMessage = "cannot both block and unblock";
                return false;
            }

            return true;
        }
    }
}
