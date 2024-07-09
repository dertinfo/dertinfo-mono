using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System.Results
{
    public class TeamCollatedFullResult
    {
        public string TeamName { get; set; }

        public IEnumerable<CompetitionEntryAttribute> TeamEntryAttributes { get; set; }
        public Dictionary<string, ScoreGroupResult> ScoreGroupResults { get; set; }
        public int danceEnteredCount { get; set; }
        public int danceCheckedCount { get; set; }
        public int danceTotalCount { get; set; }
    }
}
