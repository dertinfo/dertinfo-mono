using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionDanceSummaryDto
    {
        public int DancesCount { get; set; }
        public int DancesWithScoresTaken { get; set; }
        public int DancesWithScoresChecked { get; set; }
        public int IndividualScoresCount { get; set; }
        public int IndividualScoresTaken { get; set; }
        public int IndividualScoresChecked { get; set; }
    }
}
