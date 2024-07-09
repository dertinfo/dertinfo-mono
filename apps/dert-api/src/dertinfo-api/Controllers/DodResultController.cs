using AutoMapper;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Models.System.Enumerations;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodUsers;
using DertInfo.Services.Entity.SystemSettings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    /// <summary>
    /// A public facing controller that allows retrival of information for the Dert of Derts submissions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DodResultController : Controller
    {
        private const string _resultsPublishedKey = "SYSTEM-DOD-RESULTSPUBLISHED";
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodResultService _dodResultService;
        IDodUserService _dodUserService;
        ISystemSettingService _systemSettingService;

        public DodResultController(
            IMapper mapper,
            IDodResultService dodResultService,
            IDodUserService dodUserService,
            ISystemSettingService systemSettingService
            )
        {
            _mapper = mapper;
            _dodResultService = dodResultService;
            _dodUserService = dodUserService;
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultsPublishedSetting = await this._systemSettingService.GetByKey(_resultsPublishedKey);
            var resultsPublished = bool.Parse(resultsPublishedSetting.Value);

            if (!resultsPublished) { return BadRequest("Results not yet available"); }
            else
            {
                var teamCollatedResultPairDO = await this._dodResultService.GetOfficialAndPublicResult();

                DodTeamCollatedResultPairDto dodTeamCollatedResultPairDto = _mapper.Map<DodTeamCollatedResultPairDto>(teamCollatedResultPairDO);

                return Ok(dodTeamCollatedResultPairDto);
            }
        }



        [HttpPost]
        public async Task<IActionResult> Post(DodResultSubmissionDto dodResultSubmission)
        {
            if (!ValidDodResultSubmission(dodResultSubmission)) { return BadRequest(this._errorMessage); }

            bool userGuidPresent = dodResultSubmission.UserGuid != null && dodResultSubmission.UserGuid != string.Empty;
            bool userEmailPresent = dodResultSubmission.UserEmail != null && dodResultSubmission.UserEmail != string.Empty;

            DodUser dodUser = new DodUser();
            if (userGuidPresent && !userEmailPresent)
            {
                dodUser = await this.GetExistingUser(dodResultSubmission.UserGuid);
            }
            else
            {
                var isValidatedOfficialJudge = false;
                if (dodResultSubmission.OfficialJudge)
                {
                    isValidatedOfficialJudge = await this._dodResultService.IsOfficalJudgePasswordValid(dodResultSubmission.OfficialJudgePassword);

                    if (!isValidatedOfficialJudge) { return BadRequest("The password supplied to identify you as a judge was not valid."); }
                }

                dodUser = this.BuildNewUser(dodResultSubmission, isValidatedOfficialJudge);

                // Send to the user service to create it
                dodUser = await this._dodUserService.Create(dodUser);
            }

            // Score Information
            DodResult dodResult = new DodResult();
            dodResult.SubmissionId = dodResultSubmission.SubmissionId;
            dodResult.MusicScore = dodResultSubmission.MusicScore;
            dodResult.MusicComments = dodResultSubmission.MusicComments;
            dodResult.SteppingScore = dodResultSubmission.SteppingScore;
            dodResult.SteppingComments = dodResultSubmission.SteppingComments;
            dodResult.SwordHandlingScore = dodResultSubmission.SwordHandlingScore;
            dodResult.SwordHandlingComments = dodResultSubmission.SwordHandlingComments;
            dodResult.DanceTechniqueScore = dodResultSubmission.DanceTechniqueScore;
            dodResult.DanceTechniqueComments = dodResultSubmission.DanceTechniqueComments;
            dodResult.PresentationScore = dodResultSubmission.PresentationScore;
            dodResult.PresentationComments = dodResultSubmission.PresentationComments;
            dodResult.BuzzScore = dodResultSubmission.BuzzScore;
            dodResult.BuzzComments = dodResultSubmission.BuzzComments;
            dodResult.CharactersScore = dodResultSubmission.CharactersScore;
            dodResult.CharactersComments = dodResultSubmission.CharactersComments;
            dodResult.OverallComments = dodResultSubmission.OverallComments;
            dodResult.IsOfficial = dodUser.IsOfficial;
            dodResult.HasOutstandingComplaint = false;
            dodResult.IncludeInScores = true;
            dodResult.DodUser = dodUser; // Attach User

            dodResult = await this._dodResultService.Create(dodResult);

            DodResultDto dodResultDto = _mapper.Map<DodResultDto>(dodResult);

            return Created("api/dodresult/" + dodResultDto.Id, dodResultDto);
        }

        private DodUser BuildNewUser(DodResultSubmissionDto dodResultSubmission, bool isOfficialJudge)
        {
            // User Information
            DodUser dodUser = new DodUser();
            dodUser.Guid = Guid.NewGuid();
            dodUser.Name = dodResultSubmission.UserName;
            dodUser.Email = dodResultSubmission.UserEmail;
            dodUser.TermsAndConditionsAgreed = dodResultSubmission.AgreeToTermsAndConditions;
            dodUser.DateTermsAndConditionsAgreed = DateTime.Now;
            dodUser.IsOfficial = isOfficialJudge;
            dodUser.InterestedInJudging = dodResultSubmission.InterestedInJudging;

            return dodUser;
        }

        private async Task<DodUser> GetExistingUser(string userGuidString)
        {
            var userGuid = Guid.Parse(userGuidString);

            return await this._dodUserService.GetUserByGuid(userGuid);
        }


        private bool ValidDodResultSubmission(DodResultSubmissionDto dodResultSubmission)
        {
            if (dodResultSubmission == null)
            {
                this._errorMessage = "result submission is null";
                return false;
            }

            if (dodResultSubmission.SubmissionId == 0)
            {
                this._errorMessage = "the submission id has not been set";
                return false;
            }

            // If there is no user guid then there should be a supplied email and name and preference settings
            if (dodResultSubmission.UserGuid == null)
            {
                if (dodResultSubmission.UserName == null)
                {
                    this._errorMessage = "name has not been supplied";
                    return false;
                }

                if (dodResultSubmission.UserEmail == null)
                {
                    this._errorMessage = "email has not been supplied";
                    return false;
                }

                if (dodResultSubmission.OfficialJudge == true && (dodResultSubmission.OfficialJudgePassword == null || dodResultSubmission.OfficialJudgePassword == string.Empty))
                {
                    this._errorMessage = "official judge has been supplied as true but the password has not been supplied";
                    return false;
                }
            }

            if (dodResultSubmission.MusicScore < 0 || dodResultSubmission.MusicScore > 15)
            {
                this._errorMessage = "music score is out of range";
                return false;
            }

            if (dodResultSubmission.SteppingScore < 0 || dodResultSubmission.SteppingScore > 15)
            {
                this._errorMessage = "no stepping score is out of range";
                return false;
            }

            if (dodResultSubmission.SwordHandlingScore < 0 || dodResultSubmission.SwordHandlingScore > 15)
            {
                this._errorMessage = "no sword handling score is out of range";
                return false;
            }

            if (dodResultSubmission.DanceTechniqueScore < 0 || dodResultSubmission.DanceTechniqueScore > 15)
            {
                this._errorMessage = "no dance technique score is out of range";
                return false;
            }

            if (dodResultSubmission.PresentationScore < 0 || dodResultSubmission.PresentationScore > 15)
            {
                this._errorMessage = "no presentation score is out of range";
                return false;
            }

            if (dodResultSubmission.BuzzScore < 0 || dodResultSubmission.BuzzScore > 15)
            {
                this._errorMessage = "no buzz score is out of range";
                return false;
            }

            if (dodResultSubmission.CharactersScore < 0 || dodResultSubmission.CharactersScore > 10)
            {
                this._errorMessage = "no characters score is out of range";
                return false;
            }

            return true;
        }
    }
}
