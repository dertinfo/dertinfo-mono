using DertInfo.Models.Database;
using DertInfo.Models.System.Results;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Results
{
    public interface IResultByActivityService
    {
        Task<IOrderedEnumerable<TeamCollatedResult>> ListByScoreCategoryCollatated(int activityId, int[] scoreCategoryIds);
        Task<IOrderedEnumerable<TeamCollatedFullResult>> ListByScoreGroupCollatated(int activityId, List<ScoreGroup> scoreGroups);
        Task<IOrderedEnumerable<TeamCollatedResult>> ListByScoreCategoryFlat(int activityId, int[] scoreCategoryIds);
    }

    public class ResultByActivityService : IResultByActivityService
    {
        private IDanceRepository _danceRepository;
        private IResultByActivityCache _resultsByActivityCache;

        public ResultByActivityService(
            IDanceRepository danceRepository,
            IResultByActivityCache resultsByActivityCache
            )
        {
            _danceRepository = danceRepository;
            _resultsByActivityCache = resultsByActivityCache;
        }

        public async Task<IOrderedEnumerable<TeamCollatedResult>> ListByScoreCategoryCollatated(int activityId, int[] scoreCategoryIds)
        {
            // Do we have cache?
            var cache = await this._resultsByActivityCache.GetCache(activityId, scoreCategoryIds);
            if (cache != null) return cache.OrderByDescending(k => k.CollatedScore);

            // todo - the activity should match the score categories. Perform this check.

            var dances = await _danceRepository.GetDancesExpandedByCompetitionId(activityId);

            // todo - this needs to be completed more efficiently
            Dictionary<int, TeamCollatedResult> dictionary = new Dictionary<int, TeamCollatedResult>();

            foreach (var dance in dances)
            {
                var total = new decimal(0);

                if (!dictionary.ContainsKey(dance.TeamAttendanceId))
                {
                    TeamCollatedResult tcr = new TeamCollatedResult();
                    tcr.TeamName = dance.TeamAttendance.Team.TeamName;
                    dictionary[dance.TeamAttendanceId] = tcr;
                }

                // todo - math sum round the select
                var categoryScores = dance.DanceScores.Where(ds => scoreCategoryIds.Contains(ds.ScoreCategoryId)).Select(ds => ds.MarkGiven);
                foreach (var score in categoryScores) { total += score; }


                var activityCollatedResult = dictionary[dance.TeamAttendanceId];
                AppendDanceToAccumulation(ref activityCollatedResult, total, dance.HasScoresEntered, dance.HasScoresChecked);
                dictionary[dance.TeamAttendanceId] = activityCollatedResult;
            }

            var results = dictionary.Values.OrderByDescending(k => k.CollatedScore);

            // Create the Cache
            await this._resultsByActivityCache.CreateCache(activityId, scoreCategoryIds, results);

            return results;

        }

        public async Task<IOrderedEnumerable<TeamCollatedResult>> ListByScoreCategoryFlat(int activityId, int[] scoreCategoryIds)
        {
            // Do we have cache?
            var cache = await this._resultsByActivityCache.GetCache(activityId, scoreCategoryIds);
            if (cache != null) return cache.OrderByDescending(k => k.CollatedScore);

            var dances = await _danceRepository.GetDancesExpandedByCompetitionId(activityId);

            // note - this is not a collation as we are outputting the results of all dances individually
            List<TeamCollatedResult> allDanceResults = new List<TeamCollatedResult>();

            foreach (var dance in dances)
            {
                var total = new decimal(0);

                TeamCollatedResult tcr = new TeamCollatedResult();
                tcr.TeamName = dance.TeamAttendance.Team.TeamName;

                var categoryScores = dance.DanceScores.Where(ds => scoreCategoryIds.Contains(ds.ScoreCategoryId)).Select(ds => ds.MarkGiven);
                foreach (var score in categoryScores) { total += score; }

                tcr.CollatedScore = total;

                allDanceResults.Add(tcr);
            }

            var results = allDanceResults.OrderByDescending(k => k.CollatedScore);

            // Create the Cache
            await this._resultsByActivityCache.CreateCache(activityId, scoreCategoryIds, results);

            return results;
        }

        /// <summary>
        /// This is the builder to get the table for all teams with every score group.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="scoreGroups"></param>
        /// <returns></returns>
        public async Task<IOrderedEnumerable<TeamCollatedFullResult>> ListByScoreGroupCollatated(int activityId, List<ScoreGroup> scoreGroups)
        {
            var dances = await _danceRepository.GetDancesExpandedByCompetitionId(activityId);

            Dictionary<int, TeamCollatedFullResult> dictionary = new Dictionary<int, TeamCollatedFullResult>();

            foreach (var dance in dances)
            {

                // Create the team dictionary entry
                if (!dictionary.ContainsKey(dance.TeamAttendanceId))
                {
                    TeamCollatedFullResult tcfr = new TeamCollatedFullResult();
                    tcfr.TeamName = dance.TeamAttendance.Team.TeamName;
                    tcfr.TeamEntryAttributes = this.ExtractTeamEntryAttributes(dance); 
                    tcfr.ScoreGroupResults = new Dictionary<string, ScoreGroupResult>();
                    dictionary[dance.TeamAttendanceId] = tcfr;
                }

                TeamCollatedFullResult teamCollatedFullResult = dictionary[dance.TeamAttendanceId];

                foreach (var scoreGroup in scoreGroups)
                {

                    // Create the score group dictionary entry
                    if (!teamCollatedFullResult.ScoreGroupResults.ContainsKey(scoreGroup.ScoreGroupKey))
                    {
                        ScoreGroupResult sgr = new ScoreGroupResult();
                        sgr.CollatedScore = 0;
                        sgr.ScoreGroupKey = scoreGroup.ScoreGroupKey; // don't really need this.
                        teamCollatedFullResult.ScoreGroupResults[scoreGroup.ScoreGroupKey] = sgr;
                    }

                    ScoreGroupResult scoreGroupResult = teamCollatedFullResult.ScoreGroupResults[scoreGroup.ScoreGroupKey];

                    var danceScoreGroupTotal = dance.DanceScores.Where(ds => scoreGroup.ScoreCategoryIds.Contains(ds.ScoreCategoryId)).Select(ds => ds.MarkGiven).Sum();
                    scoreGroupResult.CollatedScore = scoreGroupResult.CollatedScore + danceScoreGroupTotal;
                    scoreGroupResult.dancesCountedChecksum++;
                }

                if (dance.HasScoresEntered) { teamCollatedFullResult.danceEnteredCount++; }
                if (dance.HasScoresChecked) { teamCollatedFullResult.danceCheckedCount++; }
                teamCollatedFullResult.danceTotalCount++;
                dictionary[dance.TeamAttendanceId] = teamCollatedFullResult;
            }

            return dictionary.Values.OrderByDescending(k => k.TeamName);
        }

        private IEnumerable<CompetitionEntryAttribute> ExtractTeamEntryAttributes(Dance dance)
        {
            var thisCompetitionCompetitionEntries = dance.TeamAttendance.CompetitionEntries.Where(ce => ce.CompetitionId == dance.CompetitionId);
            var entryAttributes = thisCompetitionCompetitionEntries.SelectMany(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries.Select(dceadce => dceadce.DertCompetitionEntryAttribute));
            return entryAttributes.Distinct();
        }

        private void AppendDanceToAccumulation(ref TeamCollatedResult activityCollatedResult, decimal valueToAdd, bool hasScoresEntered, bool hasScoresChecked)
        {
            activityCollatedResult.CollatedScore += valueToAdd;
            if (hasScoresEntered) { activityCollatedResult.danceEnteredCount++; }
            if (hasScoresChecked) { activityCollatedResult.danceCheckedCount++; }
            activityCollatedResult.danceTotalCount++;

        }
    }
}
