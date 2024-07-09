using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DataTransferObject.Results;
using DertInfo.Models.System.Results;
using DertInfo.Services;
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
    [Route("api/[controller]")]
    public class ResultsController : Controller
    {
        IMapper _mapper;
        ICompetitionService _competitionService;
        IDanceService _danceService;
        IEventSettingService _eventSettingService;
        IResultByActivityService _resultByActivityService;

        public ResultsController(
            IMapper mapper,
            ICompetitionService competitionService,
            IDanceService danceService,
            IResultByActivityService resultByActivityService,
            IEventSettingService eventSettingService
            )
        {
            _mapper = mapper;
            _competitionService = competitionService;
            _danceService = danceService;
            _eventSettingService = eventSettingService;
            _resultByActivityService = resultByActivityService;
        }

        [HttpGet]
        [Route("lookup")]
        public async Task<CompetitionLookupDto> GetLookupInfo()
        {
            var lookupsInfo = await this._competitionService.GetResultsLookupInfo();

            // Perform Map
            CompetitionLookupDto competitionLookupDto = await this.MapToCompetitionLookupDto(lookupsInfo);

            return competitionLookupDto;
        }

        /// <summary>
        /// From the compeition Id provided this function will return the collated results of the result type provided. 
        /// This runs live from data.
        /// There are predefined typeKeys of "music", "buzz", "characters" and "main" any others will include all categories. 
        /// 
        /// Consider: It might be a good idea to permit this enpoint to take the category Tag and collate on that category. 
        /// Consider: It might also be work passing the names of the score category requested back with the result to support a more generic solution.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="typeKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{activityId}/{typeKey}")]
        public async Task<CompetitionResultDto> GetByActivityAndType(int activityId, string typeKey)
        {
            Competition competition = await _competitionService.FindById(activityId);

            // todo - we need to be able to access this endpoint if the user is the event admin. 
            if (!competition.ResultsPublished)
            {
                throw new InvalidOperationException("Cannot get results if results not published");
            }

            EventSetting buzzScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_BUZZ_SCORECATEGORY_ID);
            EventSetting musicScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_MUSIC_SCORECATEGORY_ID);
            EventSetting charactersScoreCategory = await _eventSettingService.GetByType(competition.EventId, Models.System.Enumerations.EventSettingType.MAINCOMP_CHARACTERS_SCORECATEGORY_ID);

            List<int> scoreCategories = new List<int>();

            switch (typeKey.ToLower())
            {
                case "buzz":
                    scoreCategories.Add(int.Parse(buzzScoreCategory.Value));
                    break;
                case "music":
                    scoreCategories.Add(int.Parse(musicScoreCategory.Value));
                    break;
                case "characters":
                    scoreCategories.Add(int.Parse(charactersScoreCategory.Value));
                    break;
                case "main":
                    var mainCategories = competition.ScoreCategories.Where(sc => sc.Id != int.Parse(charactersScoreCategory.Value)).Select(sc => sc.Id);
                    scoreCategories.AddRange(mainCategories);
                    break;
                default:
                    var allCategories = competition.ScoreCategories.Select(sc => sc.Id);
                    scoreCategories.AddRange(allCategories);
                    break;
            }

            CompetitionResultDto competitionResultDto = new CompetitionResultDto();
            competitionResultDto.CompetitionId = competition.Id;
            competitionResultDto.CompetitionName = competition.Name;
            competitionResultDto.ResultType = typeKey;
            competitionResultDto.ScoreCategoryIdsIncluded = scoreCategories.ToArray();

            IOrderedEnumerable<TeamCollatedResult> teamCollatedResults = null;
            if (!competition.ResultsAreCollated)
            {
                teamCollatedResults = await _resultByActivityService.ListByScoreCategoryFlat(activityId, scoreCategories.ToArray());
            }
            else
            {
                teamCollatedResults = await _resultByActivityService.ListByScoreCategoryCollatated(activityId, scoreCategories.ToArray());
            }

            List<TeamCollatedResultDto> teamCollatedResultsDto = _mapper.Map<List<TeamCollatedResultDto>>(teamCollatedResults);
            competitionResultDto.TeamCollatedResults = teamCollatedResultsDto;

            return competitionResultDto;
        }

        private async Task<CompetitionLookupDto> MapToCompetitionLookupDto(IEnumerable<Competition> competitions)
        {
            var lookupDto = new CompetitionLookupDto();
            lookupDto.Events = new List<CompetitionLookupEventDto>();

            //var events = competitions.Select(c => c.Event);

            var events = competitions
                   .GroupBy(c => c.EventId)
                   .Select(grp => grp.First().Event)
                   .ToList();

            foreach (var ev in events)
            {
                EventSetting mainCompeitionSetting = ev.EventSettings.Where(es => es.Ref == Models.System.Enumerations.EventSettingType.MAINCOMP_COMPETITION_ID.ToString()).First();

                if (competitions.Any(c => c.ResultsPublished && c.EventId == ev.Id))
                {

                    CompetitionLookupEventDto eventLookup = new CompetitionLookupEventDto();
                    eventLookup.EventName = ev.Name;
                    eventLookup.Competitions = new List<CompetitionLookupCompetitionDto>();
                    lookupDto.Events.Add(eventLookup);

                    var eventComps = competitions.Where(c => c.EventId == ev.Id && c.ResultsPublished);

                    foreach (var evc in eventComps)
                    {
                        CompetitionLookupCompetitionDto competitionLookupCompetitionDto = new CompetitionLookupCompetitionDto();
                        competitionLookupCompetitionDto.CompetitionId = evc.Id;
                        competitionLookupCompetitionDto.CompetitionName = evc.Name;
                        competitionLookupCompetitionDto.ResultTypes = new List<CompetitionLookupResultTypeDto>();
                        eventLookup.Competitions.Add(competitionLookupCompetitionDto);

                        // note - watch out here not all events will have the same attributes but hardcoded here for brevity

                        // Check if main competition for the event.
                        if (evc.Id == int.Parse(mainCompeitionSetting.Value))
                        {
                            CompetitionLookupResultTypeDto resultTypeAll = new CompetitionLookupResultTypeDto();
                            resultTypeAll.ResultTypeKey = "all";
                            resultTypeAll.ResultTypeName = "Steve Marris";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeAll);

                            CompetitionLookupResultTypeDto resultTypeMain = new CompetitionLookupResultTypeDto();
                            resultTypeMain.ResultTypeKey = "main";
                            resultTypeMain.ResultTypeName = "Main";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeMain);

                            CompetitionLookupResultTypeDto resultTypeBuzz = new CompetitionLookupResultTypeDto();
                            resultTypeBuzz.ResultTypeKey = "buzz";
                            resultTypeBuzz.ResultTypeName = "Buzz";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeBuzz);

                            CompetitionLookupResultTypeDto resultTypeCharacters = new CompetitionLookupResultTypeDto();
                            resultTypeCharacters.ResultTypeKey = "characters";
                            resultTypeCharacters.ResultTypeName = "Characters";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeCharacters);

                            CompetitionLookupResultTypeDto resultTypeMusic = new CompetitionLookupResultTypeDto();
                            resultTypeMusic.ResultTypeKey = "music";
                            resultTypeMusic.ResultTypeName = "Music";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeMusic);
                        }
                        else
                        {
                            CompetitionLookupResultTypeDto resultTypeStandard = new CompetitionLookupResultTypeDto();
                            resultTypeStandard.ResultTypeKey = "all";
                            resultTypeStandard.ResultTypeName = "All";
                            competitionLookupCompetitionDto.ResultTypes.Add(resultTypeStandard);
                        }

                    }
                }

            }

            return lookupDto;

        }
    }
}
