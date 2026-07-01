using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodSubmissions;
using DertInfo.Services.Entity.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DodSubmissionAuthController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodSubmissionService _dodSubmissionService;
        IGroupService _groupService;

        public DodSubmissionAuthController(
            IMapper mapper,
            IDodSubmissionService dodSubmissionService,
            IGroupService groupService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _groupService = groupService;
            _dodSubmissionService = dodSubmissionService;
        }

        [HttpGet]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> Get()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dodSubmissions = await _dodSubmissionService.ListAll();

            // Perform Auto Map of simple fields
            List<DodSubmissionDto> dodSubmissionDtos = _mapper.Map<List<DodSubmissionDto>>(dodSubmissions);

            return Ok(dodSubmissionDtos.OrderBy(dto => dto.GroupName));
        }

        [HttpGet]
        [Route("groups")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetGroups()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groups = await _groupService.ListAll();

            List<GroupDto> dodSubmissionDtos = _mapper.Map<List<GroupDto>>(groups);

            return Ok(dodSubmissionDtos.OrderBy(dto => dto.GroupName));
        }

        [HttpGet]
        [Route("entered/{groupId}")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> HasGroupEntered([FromRoute] int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var hasEntered = await _dodSubmissionService.HasGroupEntered(groupId);

            return Ok(hasEntered);
        }

        /// <summary>
        /// Submit a new entry for DertOfDerts
        /// </summary>
        /// <param name="dodSubmissionSubmission"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> Add([FromBody] DodSubmissionSubmissionDto dodSubmissionSubmission)
        {
            if (!ValidDodSubmissionSubmission(dodSubmissionSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            DodSubmission myDodSubmission = new DodSubmission();
            myDodSubmission.GroupId = dodSubmissionSubmission.GroupId;
            myDodSubmission.EmbedLink = dodSubmissionSubmission.EmbedLink;
            myDodSubmission.EmbedOrigin = dodSubmissionSubmission.EmbedOrigin;
            myDodSubmission.DertYearFrom = dodSubmissionSubmission.DertYearFrom;
            myDodSubmission.DertVenueFrom = dodSubmissionSubmission.DertVenueFrom;
            myDodSubmission.IsPremier = dodSubmissionSubmission.IsPremier;
            myDodSubmission.IsChampionship = dodSubmissionSubmission.IsChampionship;
            myDodSubmission.IsOpen = dodSubmissionSubmission.IsOpen;

            myDodSubmission = await this._dodSubmissionService.Create(myDodSubmission);

            DodSubmissionDto dodSubmissionDto = _mapper.Map<DodSubmissionDto>(myDodSubmission);

            return Ok(dodSubmissionDto);
        }

        /// <summary>
        /// Marks the submission as deleted 
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns>201: Submission</returns>
        [HttpDelete]
        [Route("{dodSubmissionId}")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> DeleteSubmission([FromRoute] int dodSubmissionId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var dodSubmission = await _dodSubmissionService.DeleteSubmission(dodSubmissionId);

            DodSubmissionDto dodSubmissionDto = _mapper.Map<DodSubmissionDto>(dodSubmission);

            if (dodSubmission != null)
            {
                return Ok(dodSubmissionDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateSubmission(DodSubmissionUpdateSubmissionDto dodSubmissionUpdateSubmissionDto)
        {
            if (!ValidDodSubmissionUpdateSubmission(dodSubmissionUpdateSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            DodSubmission myDodSubmission = new DodSubmission();
            myDodSubmission.Id = dodSubmissionUpdateSubmissionDto.SubmissionId;
            myDodSubmission.GroupId = dodSubmissionUpdateSubmissionDto.GroupId;
            myDodSubmission.EmbedLink = dodSubmissionUpdateSubmissionDto.EmbedLink;
            myDodSubmission.EmbedOrigin = dodSubmissionUpdateSubmissionDto.EmbedOrigin;
            myDodSubmission.DertYearFrom = dodSubmissionUpdateSubmissionDto.DertYearFrom;
            myDodSubmission.DertVenueFrom = dodSubmissionUpdateSubmissionDto.DertVenueFrom;
            myDodSubmission.IsPremier = dodSubmissionUpdateSubmissionDto.IsPremier;
            myDodSubmission.IsChampionship = dodSubmissionUpdateSubmissionDto.IsChampionship;
            myDodSubmission.IsOpen = dodSubmissionUpdateSubmissionDto.IsOpen;

            myDodSubmission = await _dodSubmissionService.Update(myDodSubmission);

            DodSubmissionDto dodSubmissionDto = _mapper.Map<DodSubmissionDto>(myDodSubmission);

            return Ok(dodSubmissionDto);
        }

        #region Submission Validation

        private bool ValidDodSubmissionSubmission(DodSubmissionSubmissionDto dodSubmissionSubmission)
        {
            if (dodSubmissionSubmission.GroupId == 0)
            {
                this._errorMessage = "A group id must be supplied";
                return false;
            }

            if (dodSubmissionSubmission.EmbedLink == string.Empty)
            {
                this._errorMessage = "embed link must be supplied";
                return false;
            }

            if (dodSubmissionSubmission.EmbedOrigin == string.Empty)
            {
                this._errorMessage = "embed origin must be supplied";
                return false;
            }

            if (dodSubmissionSubmission.DertYearFrom == string.Empty)
            {
                this._errorMessage = "you must specify which year the video was from";
                return false;
            }

            return true;
        }

        private bool ValidDodSubmissionUpdateSubmission(DodSubmissionUpdateSubmissionDto dodSubmissionUpdateSubmissionDto)
        {
            if (dodSubmissionUpdateSubmissionDto == null)
            {
                this._errorMessage = "submission is null";
                return false;
            }

            if (dodSubmissionUpdateSubmissionDto.SubmissionId == 0)
            {
                this._errorMessage = "no id passed with update";
                return false;
            }

            if (dodSubmissionUpdateSubmissionDto.GroupId == 0)
            {
                this._errorMessage = "A group id must be supplied";
                return false;
            }

            if (dodSubmissionUpdateSubmissionDto.EmbedLink == string.Empty)
            {
                this._errorMessage = "embed link must be supplied";
                return false;
            }

            if (dodSubmissionUpdateSubmissionDto.EmbedOrigin == string.Empty)
            {
                this._errorMessage = "embed origin must be supplied";
                return false;
            }

            if (dodSubmissionUpdateSubmissionDto.DertYearFrom == string.Empty)
            {
                this._errorMessage = "you must specify which year the video was from";
                return false;
            }

            return true;
        }

        #endregion
    }
}
