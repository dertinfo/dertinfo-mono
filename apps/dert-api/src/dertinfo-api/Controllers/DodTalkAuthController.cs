using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodTalks;
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
    public class DodTalkAuthController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodTalkService _dodTalkService;

        public DodTalkAuthController(
            IMapper mapper,
            IDodTalkService dodTalkService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _dodTalkService = dodTalkService;
        }

        [HttpGet]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> Get()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dodTalks = await _dodTalkService.ListAll();

            // Perform Auto Map of simple fields
            List<DodTalkDto> dodTalkDtos = _mapper.Map<List<DodTalkDto>>(dodTalks);

            return Ok(dodTalkDtos.OrderBy(dto => dto.BroadcastDateTime));
        }

        [HttpPost]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> CreateTalk(DodTalkSubmissionDto dodTalkSubmissionDto)
        {
            if (!ValidTalkSubmission(dodTalkSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            DodTalk myDodTalk = new DodTalk();
            myDodTalk.Title = dodTalkSubmissionDto.Title;
            myDodTalk.SubTitle = dodTalkSubmissionDto.SubTitle;
            myDodTalk.Description = dodTalkSubmissionDto.Description;
            myDodTalk.BroadcastDateTime = dodTalkSubmissionDto.BroadcastDateTime;
            myDodTalk.BroadcastWebLink = dodTalkSubmissionDto.BroadcastWebLink;

            myDodTalk = await this._dodTalkService.Create(myDodTalk);

            DodTalkDto dodTalkDto = _mapper.Map<DodTalkDto>(myDodTalk);

            return Ok(dodTalkDto);
        }

        [HttpPut]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateTalk(DodTalkUpdateSubmissionDto dodTalkUpdateSubmissionDto)
        {
            if (!ValidTalkUpdateSubmission(dodTalkUpdateSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            DodTalk myDodTalk = new DodTalk();
            myDodTalk.Id = dodTalkUpdateSubmissionDto.TalkId;
            myDodTalk.Title = dodTalkUpdateSubmissionDto.Title;
            myDodTalk.SubTitle = dodTalkUpdateSubmissionDto.SubTitle;
            myDodTalk.Description = dodTalkUpdateSubmissionDto.Description;
            myDodTalk.BroadcastDateTime = dodTalkUpdateSubmissionDto.BroadcastDateTime;
            myDodTalk.BroadcastWebLink = dodTalkUpdateSubmissionDto.BroadcastWebLink;

            myDodTalk = await _dodTalkService.Update(myDodTalk);

            DodTalkDto dodTalkDto = _mapper.Map<DodTalkDto>(myDodTalk);

            return Ok(dodTalkDto);
        }

        [HttpDelete]
        [Route("{dodTalkId}")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> DeleteSubmission([FromRoute] int dodTalkId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var dodTalk = await _dodTalkService.Delete(dodTalkId);

            DodTalkDto dodTalkDto = _mapper.Map<DodTalkDto>(dodTalk);

            if (dodTalk != null)
            {
                return Ok(dodTalkDto);
            }
            else
            {
                return NotFound();
            }
        }

        private bool ValidTalkSubmission(DodTalkSubmissionDto dodTalkSubmissionDto)
        {
            if (dodTalkSubmissionDto == null)
            {
                this._errorMessage = "talk submission is null";
                return false;
            }

            if (dodTalkSubmissionDto.Title == null || dodTalkSubmissionDto.Title == string.Empty)
            {
                this._errorMessage = "talk requires a title";
                return false;
            }

            if (dodTalkSubmissionDto.Description == null || dodTalkSubmissionDto.Description == string.Empty)
            {
                this._errorMessage = "talk requires a description";
                return false;
            }

            if (dodTalkSubmissionDto.BroadcastDateTime == DateTime.MinValue)
            {
                this._errorMessage = "talk requires a broadcast date time";
                return false;
            }

            return true;
        }

        private bool ValidTalkUpdateSubmission(DodTalkUpdateSubmissionDto dodTalkUpdateSubmissionDto)
        {
            if (dodTalkUpdateSubmissionDto == null)
            {
                this._errorMessage = "talk submission is null";
                return false;
            }

            if (dodTalkUpdateSubmissionDto.TalkId == 0)
            {
                this._errorMessage = "no id passed with update";
                return false;
            }

            if (dodTalkUpdateSubmissionDto.Title == null || dodTalkUpdateSubmissionDto.Title == string.Empty)
            {
                this._errorMessage = "talk requires a title";
                return false;
            }

            if (dodTalkUpdateSubmissionDto.Description == null || dodTalkUpdateSubmissionDto.Description == string.Empty)
            {
                this._errorMessage = "talk requires a description";
                return false;
            }

            if (dodTalkUpdateSubmissionDto.BroadcastDateTime == DateTime.MinValue)
            {
                this._errorMessage = "talk requires a broadcast date time";
                return false;
            }

            return true;
        }

    }
}
