using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DataTransferObject.Results;
using DertInfo.Models.System.Results;
using DertInfo.Services;
using DertInfo.Services.Entity.Activities;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.EventSettings;
using DertInfo.Services.Results;
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
    public class ResultsAuthController : ResourceAuthController
    {
        IMapper _mapper;
        IActivityService _activityService;
        ICompetitionService _competitionService;
        IDanceService _danceService;
        IEventSettingService _eventSettingService;
        IResultByActivityService _resultByActivityService;
        ITeamService _teamService;

        public ResultsAuthController(
            IAuthorizationService authorizationService,
            IMapper mapper,
            IActivityService activityService,
            ICompetitionService competitionService,
            IDanceService danceService,
            IDertInfoUser user,
            IResultByActivityService resultByActivityService,
            IEventSettingService eventSettingService,
            ITeamService teamService
            ) : base(user, authorizationService)
        {
            _mapper = mapper;
            _activityService = activityService;
            _competitionService = competitionService;
            _danceService = danceService;
            _eventSettingService = eventSettingService;
            _resultByActivityService = resultByActivityService;
            _teamService = teamService;
        }

        [HttpGet]
        [Route("{activityId}")]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = CompetitionGetFullCollatedResultsPolicy.PolicyName;

            Competition competition = await this._competitionService.GetForAuthorization(activityId);

            if (competition == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, competition))
            {
                Competition myCompetition = await _competitionService.FindById(activityId);
                EventSetting resultsPublished = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.RESULTS_PUBLISHED);

                EventSetting mainCompetitionSetting = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_COMPETITION_ID);
                EventSetting buzzScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_BUZZ_SCORECATEGORY_ID);
                EventSetting musicScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_MUSIC_SCORECATEGORY_ID);
                EventSetting charactersScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_CHARACTERS_SCORECATEGORY_ID);

                IEnumerable<int> categoriesForAll = myCompetition.ScoreCategories.Select(sc => sc.Id);
                IEnumerable<int> categoriesForMain = myCompetition.ScoreCategories.Where(sc => sc.Id != int.Parse(charactersScoreCategory.Value)).Select(sc => sc.Id);

                List<ScoreGroup> scoreGroups = new List<ScoreGroup>();

                foreach (var scoreCategory in myCompetition.ScoreCategories)
                {
                    scoreGroups.Add(new ScoreGroup
                    {
                        ScoreGroupKey = scoreCategory.Tag,
                        ScoreCategoryIds = new int[] { scoreCategory.Id },
                        ScoreGroupName = scoreCategory.Name,
                    });
                }

                if (competition.Id == int.Parse(mainCompetitionSetting.Value))
                {
                    scoreGroups.Add(new ScoreGroup
                    {
                        ScoreGroupKey = "Main",
                        ScoreCategoryIds = categoriesForMain.ToArray(),
                        ScoreGroupName = "Main",
                    });
                }

                scoreGroups.Add(new ScoreGroup
                {
                    ScoreGroupKey = "All",
                    ScoreCategoryIds = categoriesForAll.ToArray(),
                    ScoreGroupName = "All",
                });



                var teamCollatedFullResults = await _resultByActivityService.ListByScoreGroupCollatated(activityId, scoreGroups);
                List<TeamCollatedFullResultDto> teamCollatedFullResultsDto = _mapper.Map<List<TeamCollatedFullResultDto>>(teamCollatedFullResults);

                return Ok(teamCollatedFullResultsDto.OrderBy(tcfr => tcfr.TeamName));
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }


        /// <summary>
        /// This is to get the results by a team Id. In order to access this then the user should carry appropriate permission to identify themselves as having the relevant group permission. 
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("team/{teamId}/{activityId}")]
        public async Task<IActionResult> GetByTeamAndCompetition(int teamId, int activityId)
        {
            // throw new NotImplementedException("Permission required on this controller before it is used");

            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var authorisationPolicy = TeamGetResultsPolicy.PolicyName;

            Team team = await this._teamService.GetForAuthorization(teamId);

            if (team == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, team))
            {
                var activity = await _activityService.GetDetail(activityId);

                if (activity.CompetitionId != null)
                {
                    // todo - this logic should not be in the controller
                    var competition = await this._competitionService.FindById((int)activity.CompetitionId);

                    if (competition.ResultsPublished)
                    {
                        var dances = await _danceService.ListByTeamAndCompetition(teamId, (int)activity.CompetitionId);

                        List<DanceDetailDto> danceDetailDtos = _mapper.Map<List<DanceDetailDto>>(dances);

                        return Ok(danceDetailDtos);
                    }
                    else
                    {
                        return Ok(new List<DanceDetailDto>());
                    }
                    
                }
                else
                {
                    return StatusCode(400, " Activity is not a competition.");
                }

                
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }
    }
}
