using DertInfo.Models.DataTransferObject.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class TeamCollatedFullResultDto
    {
        public string TeamName { get; set; }

        public IEnumerable<CompetitionEntryAttributeDto> TeamEntryAttributes { get; set; }
        public Dictionary<string, ScoreGroupResultDto> ScoreGroupResults { get; set; }
        public int danceEnteredCount { get; set; }
        public int danceCheckedCount { get; set; }
        public int danceTotalCount { get; set; }
    }
}
