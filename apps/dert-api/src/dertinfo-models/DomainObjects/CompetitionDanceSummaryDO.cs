using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class CompetitionDanceSummaryDO
    {
        public int TotalDanceCount { get; set; }
        public int ResultsEntered { get; set; }
        public int ResultsChecked { get; set; }
        public int TotalScoresCount { get; set; }
        public int ScoresEntered { get; set; }
        public int ScoresChecked { get; set; }
    }
}
