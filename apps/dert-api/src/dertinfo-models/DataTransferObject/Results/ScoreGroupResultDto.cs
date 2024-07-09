using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Results
{
    public class ScoreGroupResultDto
    {
        public string ScoreGroupKey { get; set; }
        public decimal CollatedScore { get; set; }
        public int dancesCountedChecksum { get; set; }
    }
}
