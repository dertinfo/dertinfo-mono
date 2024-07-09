using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{
    public interface IDodResultCollator
    {
        Task<List<DodTeamCollatedResultDO>> CollateOfficialResults(IEnumerable<DodSubmission> submissions);

        Task<List<DodTeamCollatedResultDO>> CollatePublicResults(IEnumerable<DodSubmission> submissions);

    }

    public class DodResultCollator : IDodResultCollator
    {
        /// <summary>
        /// From the full set of results filters by the official ones and then forwards the collation of the filtered set.
        /// </summary>
        /// <param name="submissions">The submissions that have been supplied (expanded by group & results)</param>
        public async Task<List<DodTeamCollatedResultDO>> CollateOfficialResults(IEnumerable<DodSubmission> submissions)
        {
            return await this.CollateResultSet(submissions, true, false);
        }

        public async Task<List<DodTeamCollatedResultDO>> CollatePublicResults(IEnumerable<DodSubmission> submissions)
        {
            return await this.CollateResultSet(submissions, false, true);
        }

        /// <summary>
        /// Takes the submissions and builds the collated results structure from raw data.
        /// </summary>
        /// <param name="submissions">The submissions (expanded by group & results)</param>
        /// <param name="includeOfficial">Flag if we want to include the official results.</param>
        /// <param name="includePublic">Flag if we want to include the public results.</param>
        /// <returns>The collated results with the group information</returns>
        private async Task<List<DodTeamCollatedResultDO>> CollateResultSet(IEnumerable<DodSubmission> submissions, bool includeOfficial, bool includePublic) 
        {
            var task = Task.Run(() =>
            {
                var collatedResultSet = new List<DodTeamCollatedResultDO>();

                foreach (var submission in submissions)
                {
                    var teamCollatedResult = new DodTeamCollatedResultDO();
                    teamCollatedResult.TeamName = submission.Group.GroupName;

                    if (submission.IsPremier) { teamCollatedResult.EntryAttribute = "Premier"; }
                    if (submission.IsChampionship) { teamCollatedResult.EntryAttribute = "Championship"; }
                    if (submission.IsOpen) { teamCollatedResult.EntryAttribute = "Open"; }


                    foreach (var result in submission.DodResults)
                    {
                        // Determines should include due to official and public status
                        var shouldInclude = result.IsOfficial && includeOfficial || !result.IsOfficial && includePublic;

                        
                        if (shouldInclude && result.IncludeInScores && !result.IsDeleted)
                        {
                            // !result.IncludeInScores is when there has been a complaint and the complaint has been upheld.

                            teamCollatedResult.SteveMarrisCollatedScore += (result.MusicScore + result.SteppingScore + result.SwordHandlingScore + result.DanceTechniqueScore + result.PresentationScore + result.BuzzScore + result.CharactersScore);
                            teamCollatedResult.MainCollatedScore += (result.MusicScore + result.SteppingScore + result.SwordHandlingScore + result.DanceTechniqueScore + result.PresentationScore + result.BuzzScore);

                            teamCollatedResult.MusicCollatedScore += result.MusicScore;
                            teamCollatedResult.SteppingCollatedScore += result.SteppingScore;
                            teamCollatedResult.SwordHandlingCollatedScore += result.SwordHandlingScore;
                            teamCollatedResult.DanceTechniqueCollatedScore += result.DanceTechniqueScore;
                            teamCollatedResult.PresentationCollatedScore += result.PresentationScore;
                            teamCollatedResult.BuzzCollatedScore += result.BuzzScore;
                            teamCollatedResult.CharactersCollatedScore += result.CharactersScore;
                            teamCollatedResult.NumberOfResults++;
                        }
                    }

                    collatedResultSet.Add(teamCollatedResult);
                }

                return collatedResultSet;
            });

            return await task;
        }
    }
}
