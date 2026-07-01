using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionSettingsDto
    {
        public int CompetitionId { get; set; }
        public int NoOfJudgesPerVenue { get; set; }
        public bool ResultsPublished { get; set; }
        public bool ResultsCollated { get; set; }
        public bool InTestingMode { get; set; }
        public bool AllowAdHocDanceAddition { get; set; }
    }
}
