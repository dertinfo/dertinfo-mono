using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.Api.Filters;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DataTransferObject.Reports;
using DertInfo.Services;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.Images;
using DertInfo.Services.Entity.JudgeSlots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// todo - This controller needs to be refactored (removed and moved elsewhere) this is not restful
/// </summary>
namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class JudgeReportsController : ResourceAuthController
    {
        IMapper _mapper;

        ICompetitionService _competitionService;
        IJudgeSlotService _judgeSlotService;

        public JudgeReportsController(
            IAuthorizationService authorizationService,
            IMapper mapper,
            ICompetitionService competitionService,
            IJudgeSlotService judgeSlotService,
            IDertInfoUser user
            ) : base(user, authorizationService)
        {
            _mapper = mapper;

            _competitionService = competitionService;
            _judgeSlotService = judgeSlotService;
        }

        [HttpGet("{competitionId}/{judgeId}")]
        public async Task<IActionResult> GetJudgeRangeReport(int competitionId, int judgeId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionViewReportsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                // Validate the request
                var competitionJudges = await this._competitionService.GetJudges(competitionId);

                if (!competitionJudges.Any(cj => cj.Id == judgeId)) { throw new Exception("The requested judge did not judge this competition"); }

                // Collect the data for the judge range report
                var rangeReportDo = await this._judgeSlotService.GetRangeReport(competitionId, judgeId);

                // Map into the transmission dto
                RangeReportDto judgeRangeReportDto = _mapper.Map<RangeReportDto>(rangeReportDo);


                return Ok(judgeRangeReportDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }
    }
}
