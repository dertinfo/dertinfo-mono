using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.Paperwork;
using DertInfo.Services;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.Events;
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
    public class PaperworkController : ResourceAuthController
    {
        IMapper _mapper;
        ICompetitionService _competitionService;
        IEventService _eventService;
        IPaperworkDataService _paperworkDataService;

        public PaperworkController(
            IAuthorizationService authorizationService,
            ICompetitionService competitionService,
            IEventService eventService,
            IDertInfoUser user,
            IMapper mapper,
            IPaperworkDataService paperworkDataService
            ) : base(user, authorizationService)
        {
            _mapper = mapper;
            _competitionService = competitionService;
            _eventService = eventService;
            _paperworkDataService = paperworkDataService;
        }

        [HttpGet]
        [Route("scoresheets/{competitionId}/populated")]
        public async Task<IActionResult> GetScoreSheetsData([FromRoute] int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = PaperworkGetScoreSheetsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var scoreSheetsDOs = await this._paperworkDataService.BuildScoreSheetPopulatedData(competitionId);

                IEnumerable<ScoreSheetDto> scoreSheetDtos = _mapper.Map<IEnumerable<ScoreSheetDto>>(scoreSheetsDOs);

                return Ok(scoreSheetDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("scoresheets/{competitionId}/spares")]
        public async Task<IActionResult> GetSpareSheetsData([FromRoute] int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = PaperworkGetScoreSheetsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var scoreSheetsSpareDOs = await this._paperworkDataService.BuildScoreSheetSparesData(competitionId);

                IEnumerable<ScoreSheetDto> scoreSheetDtos = _mapper.Map<IEnumerable<ScoreSheetDto>>(scoreSheetsSpareDOs);

                return Ok(scoreSheetDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }

        }

        [HttpGet]
        [Route("signinsheets/{eventId}")]

        public async Task<IActionResult> GetSignInSheetData([FromRoute] int eventId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = PaperworkGetSignInSheetsPolicy.PolicyName;

            Event myEvent = await this._eventService.GetForAuthorization(eventId);

            if (myEvent == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, myEvent))
            {
                var signInSheetDOs = await this._paperworkDataService.BuildSignInSheetData(eventId);

                IEnumerable<SignInSheetDto> signInSheetDtos = _mapper.Map<IEnumerable<SignInSheetDto>>(signInSheetDOs);

                return Ok(signInSheetDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }

        }
    }
}
