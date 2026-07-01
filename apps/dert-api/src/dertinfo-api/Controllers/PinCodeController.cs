using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DataTransferObject;
using DertInfo.Services.Entity.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace DertInfo.Api.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    public class PinCodeController: AuthController
    {

        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IGroupService _groupService;

        public PinCodeController(IMapper mapper, IGroupService groupService, IDertInfoUser user) : base(user)
        {
            _mapper = mapper;
            _groupService = groupService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PinCodeSubmissionDto pinCodeSubmission)
        {
            if (!ValidPinCodeSubmission(pinCodeSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            bool granted = await this._groupService.ProcessMemberPermissionRequest(pinCodeSubmission.PinCode);

            if (granted)
            {
                return Ok(pinCodeSubmission);
            }
            else
            {
                return BadRequest("Pin code supplied was invalid");
            }
        }

        [HttpPost]
        [Route("generate")]
        public async Task<IActionResult> GeneratePinNumbers([FromBody] PinCodeGenerationSubmissionDto pinCodeGenerationSubmissionDto)
        {
            if (!ValidPinCodeGenerationSubmission(pinCodeGenerationSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser


            var result = await _groupService.GeneratePinCodesWhereEmpty();

            return Ok(result);
        }

        

        #region Submission Validation

        private bool ValidPinCodeSubmission(PinCodeSubmissionDto pinCodeSubmission)
        {
            if (pinCodeSubmission.PinCode == string.Empty)
            {
                this._errorMessage = "a pin muber must not be empty";
                return false;
            }

            return true;
        }

        private bool ValidPinCodeGenerationSubmission(PinCodeGenerationSubmissionDto pinCodeGenerationSubmissionDto)
        {
            if (pinCodeGenerationSubmissionDto.GenerateValidationPassword != "g3n3r4t3")
            {
                this._errorMessage = "submission pasasword is invalid";
                return false;
            }

            return true;
        }

        #endregion
    }
}
