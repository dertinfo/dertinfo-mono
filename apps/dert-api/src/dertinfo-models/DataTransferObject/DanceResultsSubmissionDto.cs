using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceResultsSubmissionDto
    {
        public int DanceId { get; set; }
        public ICollection<DanceScoreSubmissionDto> DanceScores { get; set; }
        public bool Overrun { get; set; }
    }
}
 