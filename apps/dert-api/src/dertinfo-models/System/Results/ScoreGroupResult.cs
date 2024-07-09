using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System.Results
{
    public class ScoreGroupResult
    {
        public string ScoreGroupKey { get; set; }
        public decimal CollatedScore { get; set; }
        public int dancesCountedChecksum { get; set; }

    }
}
