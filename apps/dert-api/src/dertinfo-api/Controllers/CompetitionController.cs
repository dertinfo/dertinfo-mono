using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.System.Enumerations;
using DertInfo.Services;
using DertInfo.Services.Entity.Competitions;
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
    public class CompetitionController : ResourceAuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        ICompetitionService _competitionService;

        public CompetitionController(
            IAuthorizationService authorizationService,
            ICompetitionService competitionService,
            IMapper mapper,
            IDertInfoUser user
            ) : base(user, authorizationService)
        {
            _mapper = mapper;
            _competitionService = competitionService;
        }

        #region GET

        [HttpGet("{competitionId}/overview")]
        public async Task<IActionResult> GetOverview(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetSummaryPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var myCompetition = await this._competitionService.GetForSummary(competitionId);

                CompetitionSummaryDto competitionSummaryDto = _mapper.Map<CompetitionSummaryDto>(myCompetition);

                // note this building mechanism is not correct and should not be replicated.
                CompetitionOverviewDto overviewDto = new CompetitionOverviewDto();
                overviewDto.Id = competitionSummaryDto.CompetitionId;
                overviewDto.Name = competitionSummaryDto.CompetitionName;
                overviewDto.Summary = competitionSummaryDto;

                return Ok(overviewDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/settings")]
        public async Task<IActionResult> GetSettings(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetSettingsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var myCompetition = await this._competitionService.GetForSettings(competitionId);

                CompetitionSettingsDto competitionSettingsDto = _mapper.Map<CompetitionSettingsDto>(myCompetition);

                return Ok(competitionSettingsDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/scoresets")]
        public async Task<IActionResult> GetScoreSets(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetScoringInfoPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var scoreSets = await this._competitionService.GetScoreSets(competitionId);

                // Perform Auto Map of simple fields
                List<ScoreSetDto> scoreSetDtos = _mapper.Map<List<ScoreSetDto>>(scoreSets);

                return Ok(scoreSetDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/scorecategories")]
        public async Task<IActionResult> GetScoreCategories(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetScoringInfoPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var scoreSets = await this._competitionService.GetScoreCategories(competitionId);

                // Perform Auto Map of simple fields
                List<ScoreCategoryDto> scoreSetDtos = _mapper.Map<List<ScoreCategoryDto>>(scoreSets);

                return Ok(scoreSetDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/venues")]
        public async Task<IActionResult> GetVenues(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetVenuesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var venues = await this._competitionService.GetVenues(competitionId);

                // Perform Auto Map of simple fields
                List<VenueAllocationDto> venueDtos = _mapper.Map<List<VenueAllocationDto>>(venues);

                return Ok(venueDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/judges")]
        public async Task<IActionResult> GetJudges(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetJudgesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var judges = await this._competitionService.GetJudges(competitionId);

                // Perform Auto Map of simple fields
                List<JudgeDto> judgesDtos = _mapper.Map<List<JudgeDto>>(judges);

                return Ok(judgesDtos.OrderBy(jDto => jDto.Name));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/entryattributes")]
        public async Task<IActionResult> GetEntryAttributes(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetEntryAttributesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var entryAttributes = await this._competitionService.GetEntryAttributes(competitionId);

                // Perform Auto Map of simple fields
                List<CompetitionEntryAttributeDto> entryAttributeDtos = _mapper.Map<List<CompetitionEntryAttributeDto>>(entryAttributes);

                return Ok(entryAttributeDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/entrants")]
        public async Task<IActionResult> GetEntrants(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetEntrantsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                IEnumerable<CompetitionTeamEntryDO> entrants = await this._competitionService.GetEntrants(competitionId);

                // Perform Auto Map of simple fields
                List<GroupTeamCompetitionEntryDto> entrantDtos = _mapper.Map<List<GroupTeamCompetitionEntryDto>>(entrants);

                return Ok(entrantDtos.OrderBy(eDto => eDto.TeamName));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/dances")]
        public async Task<IActionResult> GetDances(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetDancesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var dances = await this._competitionService.GetDances(competitionId);

                // Perform Auto Map of simple fields
                List<DanceDetailDto> danceDetailDtos = _mapper.Map<List<DanceDetailDto>>(dances);

                return Ok(danceDetailDtos.OrderBy(dd => dd.TeamName));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/lookup/judges")]
        public async Task<IActionResult> GetJudgesLookup(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = LookupJudgesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var judges = await this._competitionService.LookupAllJudges(competitionId);

                // Perform Auto Map of simple fields
                List<JudgeDto> judgeDtos = _mapper.Map<List<JudgeDto>>(judges);

                return Ok(judgeDtos.OrderBy(jDto => jDto.Name));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{competitionId}/lookup/venues")]
        public async Task<IActionResult> GetVenuesLookup(int competitionId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = LookupVenuesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var venues = await this._competitionService.LookupAllVenues(competitionId);

                // Perform Auto Map of simple fields
                List<VenueDto> venuesDtos = _mapper.Map<List<VenueDto>>(venues);

                return Ok(venuesDtos.OrderBy(jDto => jDto.Name));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region PUT

        [HttpPut]
        [Route("{competitionId}/settings")]
        public async Task<IActionResult> UpdateSettings([FromRoute] int competitionId, [FromBody] CompetitionSettingsUpdateSubmissionDto competitionSettingsUpdate)
        {
            var authorisationPolicy = CompetitionUpdateSettingsPolicy.PolicyName;

            if (!ValidCompetitionSettingsUpdate(competitionSettingsUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                Competition myCompetition = new Competition();
                myCompetition.Id = competition.Id;
                myCompetition.JudgeRequirementPerVenue = competitionSettingsUpdate.NoOfJudgesPerVenue;
                myCompetition.ResultsPublished = competitionSettingsUpdate.ResultsPublished;
                myCompetition.ResultsAreCollated = competitionSettingsUpdate.ResultsCollated;
                myCompetition.InTestingMode = competitionSettingsUpdate.InTestingMode;
                myCompetition.AllowAdHocDanceAddition = competitionSettingsUpdate.AllowAdHocDanceAddition;


                await this._competitionService.UpdateCompetitionSettings(myCompetition);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/scoreset/{scoreSetId}")]
        public async Task<IActionResult> UpdateScoreSet([FromRoute] int competitionId, [FromRoute] int scoreSetId, [FromBody] ScoreSetUpdateSubmissionDto scoreSetUpdate)
        {
            var authorisationPolicy = CompetitionUpdateScoringInfoPolicy.PolicyName;

            if (!ValidScoreSetUpdate(scoreSetUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                ScoreSet scoreSet = new ScoreSet();
                scoreSet.CompetitionId = competitionId;
                scoreSet.Id = scoreSetId;
                scoreSet.Name = scoreSetUpdate.Name;

                await this._competitionService.UpdateScoreSet(scoreSet);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/scorecategory/{scoreCategoryId}")]
        public async Task<IActionResult> UpdateScoreCategory([FromRoute] int competitionId, [FromRoute] int scoreCategoryId, [FromBody] ScoreCategoryUpdateSubmissionDto scoreCategoryUpdate)
        {

            var authorisationPolicy = CompetitionUpdateScoringInfoPolicy.PolicyName;

            if (!ValidScoreCategoryUpdate(scoreCategoryUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                ScoreCategory scoreCategory = new ScoreCategory();
                scoreCategory.CompetitionAppliesToId = competitionId;
                scoreCategory.Id = scoreCategoryId;
                scoreCategory.Name = scoreCategoryUpdate.Name;
                scoreCategory.MaxMarks = scoreCategoryUpdate.MaxMarks;
                scoreCategory.Tag = scoreCategoryUpdate.Tag;
                scoreCategory.SortOrder = scoreCategoryUpdate.SortOrder;



                await this._competitionService.UpdateScoreCategory(scoreCategory);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/venue/{venueId}")]
        public async Task<IActionResult> UpdateVenue([FromRoute] int competitionId, [FromRoute] int venueId, [FromBody] VenueUpdateSubmissionDto venueUpdate)
        {
            var authorisationPolicy = CompetitionUpdateVenuePolicy.PolicyName;

            if (!ValidVenueUpdate(venueUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                Venue venue = new Venue();
                venue.Id = venueId;
                venue.Name = venueUpdate.Name;

                await this._competitionService.UpdateVenue(venue);

                foreach (var judgeSlotUpdate in venueUpdate.JudgeSlotUpdates)
                {
                    JudgeSlot judgeSlot = new JudgeSlot();
                    judgeSlot.Id = judgeSlotUpdate.Id;
                    judgeSlot.JudgeId = judgeSlotUpdate.JudgeId;

                    await this._competitionService.UpdateJudgeSlot(judgeSlot);
                }

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/judge/{judgeId}")]
        public async Task<IActionResult> UpdateJudge([FromRoute] int competitionId, [FromRoute] int judgeId, [FromBody] JudgeUpdateSubmissionDto judgeUpdate)
        {
            var authorisationPolicy = CompetitionUpdateJudgePolicy.PolicyName;

            if (!ValidJudgeUpdate(judgeUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                Judge judge = new Judge();
                judge.Id = judgeId;
                judge.Name = judgeUpdate.Name;
                judge.Telephone = judgeUpdate.Telephone;
                judge.Email = judgeUpdate.Email;

                await this._competitionService.UpdateJudge(judge);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/entrant/{competitionEntryId}/attribute")]
        public async Task<IActionResult> UpdateEntrantWithAttribute([FromRoute] int competitionId, [FromRoute] int competitionEntryId, [FromBody] CompetitionAttachEntryAttributeDto attachSubmission)
        {
            var authorisationPolicy = CompetitionUpdateEntrantAttributesPolicy.PolicyName;

            if (!ValidEntryAttributeAttach(attachSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var competitionEntry = await this._competitionService.AttachEntryAttribute(competitionId, competitionEntryId, attachSubmission.EntryAttributeId);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/reset")]
        public async Task<IActionResult> ResetCompetition([FromRoute] int competitionId)
        {
            var authorisationPolicy = CompetitionResetPolicy.PolicyName;

            // note - no submission at this time. We may want to add a submission to allow switches to state whether this is to be a full replacement or partial update.
            //if (!ValidJudgeSubmission(judgeSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                switch (competition.FlowState)
                {
                    case CompetitionFlowState.Generated:
                        await _competitionService.ClearCompetitionDances(competitionId);
                        await _competitionService.ResetCompetitionAttendance(competitionId);
                        await _competitionService.ApplyCompetitionDances(competitionId);
                        break;
                    case CompetitionFlowState.Populated:
                        await _competitionService.ResetCompetitionAttendance(competitionId);
                        break;
                }

                // note - this feels a little badly names being used in this context. Should be more generic e.g. CompetitionSummary
                EventCompetitionDto eventCompetitionDto = _mapper.Map<EventCompetitionDto>(competition);

                return NoContent();

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/populate")]
        public async Task<IActionResult> PopulateEntrants([FromRoute] int competitionId)
        {
            var authorisationPolicy = CompetitionPopulateEntrantsPolicy.PolicyName;

            // note - no submission at this time. We may want to add a submission to allow switches to state whether this is to be a full replacement or partial update.
            //if (!ValidJudgeSubmission(judgeSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                await _competitionService.ApplyCompetitionAttendance(competitionId);

                return NoContent();

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{competitionId}/generatedances")]
        public async Task<IActionResult> GenerateDances([FromRoute] int competitionId)
        {
            var authorisationPolicy = CompetitionGenerateDancesPolicy.PolicyName;

            // note - no submission at this time. We may want to add a submission to allow switches to state whether this is to be a full replacement or partial update.
            //if (!ValidJudgeSubmission(judgeSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                await _competitionService.ApplyCompetitionDances(competitionId);

                return NoContent();

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region POST

        [HttpPost]
        [Route("{competitionId}/venue")]
        public async Task<IActionResult> AddVenue([FromRoute] int competitionId, [FromBody] VenueSubmissionDto venueSubmission)
        {
            var authorisationPolicy = CompetitionAddVenuePolicy.PolicyName;

            if (!ValidVenueSubmission(venueSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                Venue myVenue = new Venue();
                myVenue.EventId = competition.EventId;
                myVenue.Name = venueSubmission.Name;

                myVenue = await _competitionService.CreateVenue(competitionId, myVenue);

                VenueAllocationDto venueAllocationDto = _mapper.Map<VenueAllocationDto>(myVenue);

                return Created("api/competition/" + competitionId + "/venue/" + myVenue.Id, venueAllocationDto);

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/judge")]
        public async Task<IActionResult> AddJudge([FromRoute] int competitionId, [FromBody] JudgeSubmissionDto judgeSubmission)
        {
            var authorisationPolicy = CompetitionAddJudgePolicy.PolicyName;

            if (!ValidJudgeSubmission(judgeSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                Judge myJudge = new Judge();
                myJudge.Name = judgeSubmission.Name;
                myJudge.Telephone = judgeSubmission.Telephone;
                myJudge.Email = judgeSubmission.Email;

                myJudge = await _competitionService.CreateJudge(competitionId, myJudge);

                JudgeDto judgeDto = _mapper.Map<JudgeDto>(myJudge);

                return Created("api/competition/" + competitionId + "/judge/" + myJudge.Id, judgeDto);

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/dance")]
        public async Task<IActionResult> AddDance([FromRoute] int competitionId, [FromBody] DanceAdditionSubmissionDto danceAdditionSubmission)
        {
            var authorisationPolicy = CompetitionAddDancePolicy.PolicyName;

            if (!ValidDanceAdditionSubmission(danceAdditionSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {

                var myDance = await _competitionService.AddAdHocDance(competitionId, danceAdditionSubmission.CompetitionEntryId, danceAdditionSubmission.VenueId);

                DanceDetailDto danceDetailDto = _mapper.Map<DanceDetailDto>(myDance);

                return Created("api/competition/" + competitionId + "/dance/" + myDance.Id, danceDetailDto);

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/judges/attach")]
        public async Task<IActionResult> AttachJudges([FromRoute] int competitionId, [FromBody] CompetitionAttachJudgesSubmissionDto attachJudgesSubmission)
        {
            var authorisationPolicy = CompetitionChangeJudgesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                base.ExtractUser(); // Fill the scoped injected IDertInfoUser

                var judgeIds = attachJudgesSubmission.JudgeIds;

                var judges = await _competitionService.AttachJudges(competitionId, judgeIds);

                IEnumerable<JudgeDto> judgeDtos = _mapper.Map<IEnumerable<JudgeDto>>(judges);

                return Ok(judgeDtos);

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/judges/detach")]
        public async Task<IActionResult> DetachJudges([FromRoute] int competitionId, [FromBody] CompetitionDetachJudgesSubmissionDto detachJudgesSubmission)
        {
            var authorisationPolicy = CompetitionChangeJudgesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                base.ExtractUser(); // Fill the scoped injected IDertInfoUser

                var judgeIds = detachJudgesSubmission.JudgeIds;

                var judges = await _competitionService.DetachJudges(competitionId, judgeIds);

                IEnumerable<JudgeDto> judgeDtos = _mapper.Map<IEnumerable<JudgeDto>>(judges);

                return Ok(judgeDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/venues/attach")]
        public async Task<IActionResult> AttachVenues([FromRoute] int competitionId, [FromBody] CompetitionAttachVenuesSubmissionDto attachVenuesSubmission)
        {
            var authorisationPolicy = CompetitionChangeVenuesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                base.ExtractUser(); // Fill the scoped injected IDertInfoUser

                var venuesIds = attachVenuesSubmission.VenueIds;

                var venues = await _competitionService.AttachVenues(competitionId, venuesIds);

                IEnumerable<VenueAllocationDto> venueDtos = _mapper.Map<IEnumerable<VenueAllocationDto>>(venues);

                return Ok(venueDtos);

            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{competitionId}/venues/detach")]
        public async Task<IActionResult> DetachVenues([FromRoute] int competitionId, [FromBody] CompetitionDetachVenuesSubmissionDto detachVenuesSubmission)
        {
            var authorisationPolicy = CompetitionChangeVenuesPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                base.ExtractUser(); // Fill the scoped injected IDertInfoUser

                var venueIds = detachVenuesSubmission.VenueIds;

                var venues = await _competitionService.DetachVenues(competitionId, venueIds);

                IEnumerable<VenueAllocationDto> venueDtos = _mapper.Map<IEnumerable<VenueAllocationDto>>(venues);

                return Ok(venueDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region DELETE

        [HttpDelete]
        [Route("{competitionId}/entrant/{competitionEntryId}/attribute/{entryAttributeId}")]
        public async Task<IActionResult> UpdateEntrantWithAttribute([FromRoute] int competitionId, [FromRoute] int competitionEntryId, [FromRoute] int entryAttributeId)
        {
            var authorisationPolicy = CompetitionUpdateEntrantAttributesPolicy.PolicyName;

            if (!ValidEntryAttributeDetach(entryAttributeId)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Competition competition = await this._competitionService.GetForAuthorization(competitionId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                var competitionEntry = await this._competitionService.DetachEntryAttribute(competitionId, competitionEntryId, entryAttributeId);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion 

        #region Validation

        private bool ValidCompetitionSettingsUpdate(CompetitionSettingsUpdateSubmissionDto competitionSettingsUpdate)
        {
            if (competitionSettingsUpdate.NoOfJudgesPerVenue == 0)
            {
                this._errorMessage = "You cannot have a competition with 0 judges per venue";
                return false;
            }

            return true;
        }

        private bool ValidScoreSetUpdate(ScoreSetUpdateSubmissionDto scoreSetUpdate)
        {
            if (scoreSetUpdate.Name == string.Empty)
            {
                this._errorMessage = "You must provide a name for the score set";
                return false;
            }

            return true;
        }

        private bool ValidScoreCategoryUpdate(ScoreCategoryUpdateSubmissionDto scoreCategoryUpdate)
        {
            if (scoreCategoryUpdate.Name == string.Empty)
            {
                this._errorMessage = "You must provide a name for the score category";
                return false;
            }

            if (scoreCategoryUpdate.MaxMarks <= 0)
            {
                this._errorMessage = "There most be some marks for a score category";
                return false;
            }

            if (scoreCategoryUpdate.Tag == string.Empty)
            {
                this._errorMessage = "You must provide a tag for the score category";
                return false;
            }

            if (scoreCategoryUpdate.SortOrder < 0)
            {
                this._errorMessage = "Sort order must be positive";
                return false;
            }

            return true;
        }

        private bool ValidVenueUpdate(VenueUpdateSubmissionDto venueUpdate)
        {
            if (venueUpdate.Name == string.Empty)
            {
                this._errorMessage = "You cannot have a venue without a name";
                return false;
            }

            return true;
        }

        private bool ValidVenueSubmission(VenueSubmissionDto venueSubmission)
        {
            if (venueSubmission.Name == string.Empty)
            {
                this._errorMessage = "You cannot have a venue without a name";
                return false;
            }

            return true;
        }

        private bool ValidJudgeUpdate(JudgeUpdateSubmissionDto judgeUpdate)
        {
            if (judgeUpdate.Name == string.Empty)
            {
                this._errorMessage = "You cannot have a judge without a name";
                return false;
            }

            return true;
        }

        private bool ValidJudgeSubmission(JudgeSubmissionDto judgeSubmission)
        {
            if (judgeSubmission.Name == string.Empty)
            {
                this._errorMessage = "You cannot have a judge without a name";
                return false;
            }

            return true;
        }
        private bool ValidEntryAttributeAttach(CompetitionAttachEntryAttributeDto attachSubmission)
        {
            if (attachSubmission.EntryAttributeId == 0)
            {
                this._errorMessage = "Invalid attribute Id supplied:" + attachSubmission.EntryAttributeId;
                return false;
            }

            return true;
        }

        private bool ValidEntryAttributeDetach(int entryAttributeId)
        {
            if (entryAttributeId == 0)
            {
                this._errorMessage = "Invalid attribute Id supplied:" + entryAttributeId;
                return false;
            }

            return true;
        }


        private bool ValidDanceAdditionSubmission(DanceAdditionSubmissionDto danceAdditionSubmission)
        {
            if (danceAdditionSubmission.CompetitionEntryId == 0)
            {
                this._errorMessage = "Invalid competition entry id supplied:" + danceAdditionSubmission.CompetitionEntryId;
                return false;
            }

            if (danceAdditionSubmission.VenueId == 0)
            {
                this._errorMessage = "Invalid venue id supplied:" + danceAdditionSubmission.VenueId;
                return false;
            }

            return true;
        }



        #endregion
    }
}
