using AutoMapper;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodUsers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DodUserController : Controller
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodResultService _dodResultService;
        IDodUserService _dodUserService;

        public DodUserController(
            IMapper mapper,
            IDodResultService dodResultService,
            IDodUserService dodUserService
            )
        {
            _mapper = mapper;
            _dodResultService = dodResultService;
            _dodUserService = dodUserService;
        }

        [HttpPost]
        [Route("recoversession")]
        public async Task<IActionResult> RecoverSession(DodRecoverSessionSubmissionDto dodRecoverSessionSubmissionDto)
        {
            if (!ValidRecoverSessionSubmission(dodRecoverSessionSubmissionDto)) { return BadRequest(this._errorMessage); }

            DodUser dodUser = new DodUser();

            var userGuid = Guid.Parse(dodRecoverSessionSubmissionDto.UserGuid);

            var foundUser = await this._dodUserService.GetUserByGuid(userGuid);
            if (foundUser != null)
            {
                if (foundUser.RecoveryPermittedUntil < DateTime.Now)
                {
                    return Forbid();
                }

                if (foundUser.Email == dodRecoverSessionSubmissionDto.UserEmail)
                {
                    dodUser = foundUser;

                    var dancesJudged = await this._dodResultService.GetJudgedDancesByUserId(foundUser.Id);

                    DodRecoverSessionDto recoverSessionDto = new DodRecoverSessionDto();
                    recoverSessionDto.UserName = foundUser.Name;
                    recoverSessionDto.OfficialJudge = foundUser.IsOfficial;
                    recoverSessionDto.DancesJudged = dancesJudged.ToList();

                    await this._dodUserService.ExtendRecovery(foundUser);

                    return Ok(recoverSessionDto);
                }
                else
                {
                    return BadRequest("Email supplied does not match that on record");
                }
            }

            return BadRequest("User not found");
        }

        [HttpPost]
        [Route("identifyjudge")]
        public async Task<IActionResult> IdentifyJudge(DodIdentifyJudgeSubmissionDto dodIdentifyJudgeSubmissionDto)
        {
            if (!ValidIdentifyJudgeSubmission(dodIdentifyJudgeSubmissionDto)) { return BadRequest(this._errorMessage); }

            var isValidatedOfficialJudge = await this._dodResultService.IsOfficalJudgePasswordValid(dodIdentifyJudgeSubmissionDto.JudgePassword);

            if (isValidatedOfficialJudge)
            {
                DodUser dodUser = new DodUser();

                // User Information
                dodUser.Guid = Guid.NewGuid();
                dodUser.Name = dodIdentifyJudgeSubmissionDto.UserName;
                dodUser.Email = dodIdentifyJudgeSubmissionDto.UserEmail;
                dodUser.TermsAndConditionsAgreed = dodIdentifyJudgeSubmissionDto.AgreeToTermsAndConditions;
                dodUser.DateTermsAndConditionsAgreed = DateTime.Now;
                dodUser.IsOfficial = isValidatedOfficialJudge;
                dodUser.ResultsBlocked = false;
                dodUser.RecoveryPermittedUntil = DateTime.Now.AddDays(6);

                await this._dodUserService.Create(dodUser);

                var responseDto = new DodIdentifyJudgeSubmissionResponseDto()
                {
                    UserGuid = dodUser.Guid,
                    IsOfficialJudge = isValidatedOfficialJudge
                };

                return Ok(responseDto);
            }
            else
            {
                return BadRequest("Judge password is invalid. The information provided does not allow us to identify you as an official judge.");
            }
        }

        private bool ValidRecoverSessionSubmission(DodRecoverSessionSubmissionDto dodRecoverSessionSubmissionDto)
        {
            if (dodRecoverSessionSubmissionDto == null)
            {
                this._errorMessage = "recover session submission is null";
                return false;
            }

            if (dodRecoverSessionSubmissionDto.UserGuid == null || dodRecoverSessionSubmissionDto.UserGuid == string.Empty)
            {
                this._errorMessage = "user guid has not been supplied";
                return false;
            }

            if (dodRecoverSessionSubmissionDto.UserGuid == null)
            {
                this._errorMessage = "user guid has not been supplied";
                return false;
            }

            if (dodRecoverSessionSubmissionDto.UserEmail == null)
            {
                this._errorMessage = "email has not been supplied";
                return false;
            }

            return true;
        }

        private bool ValidIdentifyJudgeSubmission(DodIdentifyJudgeSubmissionDto dodIdentifyJudgeSubmissionDto)
        {
            if (dodIdentifyJudgeSubmissionDto == null)
            {
                this._errorMessage = "recover session submission is null";
                return false;
            }


            if (dodIdentifyJudgeSubmissionDto.UserEmail == null)
            {
                this._errorMessage = "email has not been supplied";
                return false;
            }

            if (dodIdentifyJudgeSubmissionDto.UserName == null)
            {
                this._errorMessage = "name has not been supplied";
                return false;
            }

            if (dodIdentifyJudgeSubmissionDto.JudgePassword == null)
            {
                this._errorMessage = "judge password has not been supplied";
                return false;
            }

            if (dodIdentifyJudgeSubmissionDto.AgreeToTermsAndConditions == false)
            {
                this._errorMessage = "terms and conditions have not been agreed";
                return false;
            }

            return true;
        }
    }
}
