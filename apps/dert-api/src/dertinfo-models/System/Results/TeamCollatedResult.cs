using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System.Results
{
    public class TeamCollatedResult
    {
        public string TeamName { get; set; }
        public decimal CollatedScore { get; set; }
        public int danceEnteredCount { get; set; }
        public int danceCheckedCount { get; set; }
        public int danceTotalCount { get; set; }

    }
}
