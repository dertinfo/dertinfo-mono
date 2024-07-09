using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Models.System.Enumerations;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodUsers;
using DertInfo.Services.Entity.SystemSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    /// <summary>
    /// A public facing controller that allows retrival of information for the Dert of Derts submissions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DodResultAuthController : AuthController
    {
        private string _errorMessage = string.Empty;
        private const string _dodResultsPublishedKey = "SYSTEM-DOD-RESULTSPUBLISHED";
        private const string _dodPublicResultsForwardedKey = "SYSTEM-DOD-PUBLICRESULTSFORWARDED";
        private const string _dodOfficialResultsForwardedKey = "SYSTEM-DOD-OFFICIALRESULTSFORWARDED";

        IMapper _mapper;
        IDodResultService _dodResultService;
        IDodUserService _dodUserService;
        ISystemSettingService _systemSettingsService;

        public DodResultAuthController(
            IMapper mapper,
            IDodResultService dodResultService,
            IDodUserService dodUserService,
            ISystemSettingService systemSettingsService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _dodResultService = dodResultService;
            _dodUserService = dodUserService;
            _systemSettingsService = systemSettingsService;
        }

        [HttpGet]
        [Route("scorecards/group/{groupId}")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> GetForGroup([FromRoute] int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var resultsPublishedSetting = await this._systemSettingsService.GetByKey(_dodResultsPublishedKey);
            var publicResultsForwardedSetting = await this._systemSettingsService.GetByKey(_dodPublicResultsForwardedKey);
            var officalResultsForwardedSetting = await this._systemSettingsService.GetByKey(_dodOfficialResultsForwardedKey);

            var resultsPublished = bool.Parse(resultsPublishedSetting.Value);
            var publicResultsForwarded = bool.Parse(publicResultsForwardedSetting.Value);
            var officalResultsForwarded = bool.Parse(officalResultsForwardedSetting.Value);

            // Get all the results
            var groupResultsDo = await this._dodResultService.GetResultsForGroup(groupId);
            var removePublicScoreCards = !resultsPublished && !publicResultsForwarded;
            var removeOfficalScoreCards = !resultsPublished && !officalResultsForwarded;

            if (removePublicScoreCards && removeOfficalScoreCards)
            {
                groupResultsDo.ScoreCards = new List<Models.DomainObjects.DertOfDerts.DodGroupResultsScoreCardDO>();
            }

            
            // Filter the results to only contain the offical ones
            if (removePublicScoreCards)
            {
                groupResultsDo.ScoreCards = groupResultsDo.ScoreCards.Where(sc => sc.IsOfficial).ToList();
            }

            // Filter the results to only contain public ones
            if (removeOfficalScoreCards)
            {
                groupResultsDo.ScoreCards = groupResultsDo.ScoreCards.Where(sc => !sc.IsOfficial).ToList();
            }

            DodGroupResultsDto groupResultsDto = _mapper.Map<DodGroupResultsDto>(groupResultsDo);

            return Ok(groupResultsDto);
        }

        [HttpGet]
        [Route("scorecards/judge/{judgeId}")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetForJudge([FromRoute] int judgeId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupResultsDo = await this._dodResultService.GetResultsForJudge(judgeId);

            DodUserResultsDto groupResultsDto = _mapper.Map<DodUserResultsDto>(groupResultsDo);

            return Ok(groupResultsDto);

        }

        [HttpGet]
        [Route("scorecards/submission/{submissionId}")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetForSubmission([FromRoute] int submissionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupResultsDo = await this._dodResultService.GetResultsForSubmission(submissionId);

            DodGroupResultsDto groupResultsDto = _mapper.Map<DodGroupResultsDto>(groupResultsDo);

            return Ok(groupResultsDto);

        }

        [HttpGet]
        [Route("opencomplaints")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetOpenComplaints([FromRoute] int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dodResultComplaints = await this._dodResultService.GetOpenComplaints();

            List<DodResultComplaintDto> dodResultComplaintDtos = _mapper.Map<List<DodResultComplaintDto>>(dodResultComplaints);

            return Ok(dodResultComplaintDtos);

        }

        [HttpGet]
        [Route("closedcomplaints")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetClosedComplaints([FromRoute] int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dodResultComplaints = await this._dodResultService.GetClosedComplaints();

            List<DodResultComplaintDto> dodResultComplaintDtos = _mapper.Map<List<DodResultComplaintDto>>(dodResultComplaints);

            return Ok(dodResultComplaintDtos);

        }

        /// <summary>
        /// Update the complaint by setting validated and resolved. Removes the score from the listings and the calculations.
        /// </summary>
        /// <remarks>
        /// PUT used as this action should be immutable.
        /// </remarks>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("validatecomplaint")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> ValidateComplaint([FromBody] DodResultValidateComplaintSubmissionDto dodResultComplaintUpdateSubmissionDto)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var succeeded = await this._dodResultService.ValidateComplaint(dodResultComplaintUpdateSubmissionDto.DodResultComplaintId);

            return Ok(succeeded);
        }

        /// <summary>
        /// Update the complaint by setting validated and resolved. Removes the score from the listings and the calculations.
        /// </summary>
        /// <remarks>
        /// PUT used as this action should be immutable.
        /// </remarks>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("rejectcomplaint")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> RejectComplaint([FromBody] DodResultRejectComplaintSubmissionDto dodResultComplaintUpdateSubmissionDto)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var succeeded = await this._dodResultService.RejectComplaint(dodResultComplaintUpdateSubmissionDto.DodResultComplaintId);

            return Ok(succeeded);
        }

        [HttpPost]
        [Route("group/{groupId}/addcomplaint")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> AddResultComplaint([FromRoute] int groupId, [FromBody] DodResultComplaintSubmissionDto dodResultComplaintSubmissionDto)
        {
            if (!ValidDodResultComplaintSubmission(dodResultComplaintSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var result = await this._dodResultService.GetResultById(dodResultComplaintSubmissionDto.ResultId);

            if (groupId != result.Submission.GroupId) { return BadRequest("Complaints cannot be created against results other if you are not a member of that group"); }

            DodResultComplaint myDodResultComplaint = new DodResultComplaint();
            myDodResultComplaint.DodResultId = dodResultComplaintSubmissionDto.ResultId;
            myDodResultComplaint.ForScores = dodResultComplaintSubmissionDto.ForScores;
            myDodResultComplaint.ForComments = dodResultComplaintSubmissionDto.ForComments;
            myDodResultComplaint.IsResolved = false;
            myDodResultComplaint.Notes = dodResultComplaintSubmissionDto.Notes;

            var suceeded = await this._dodResultService.CreateComplaint(myDodResultComplaint);

            return Ok(suceeded);

        }

        [HttpGet]
        [Route("clearcache")]
        [Authorize(Policy = "DodAdministratorOnlyPolicy")]
        public async Task<IActionResult> ClearResultCache()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var suceeded = await this._dodResultService.ClearResultCache();

            return Ok(suceeded);
        }

        private bool ValidDodResultComplaintSubmission(DodResultComplaintSubmissionDto dodResultComplaintSubmissionDto)
        {
            if (dodResultComplaintSubmissionDto == null)
            {
                this._errorMessage = "complaint submission is null";
                return false;
            }

            if (dodResultComplaintSubmissionDto.ResultId == 0)
            {
                this._errorMessage = "complaint submission does not have result id set";
                return false;
            }

            if (dodResultComplaintSubmissionDto.Notes == null || dodResultComplaintSubmissionDto.Notes == string.Empty)
            {
                this._errorMessage = "complaint submission does not have any notes";
                return false;
            }

            return true;
        }
    }
}
