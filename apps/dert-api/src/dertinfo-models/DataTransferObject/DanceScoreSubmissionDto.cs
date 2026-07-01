using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceScoreSubmissionDto
    {
        public int DanceId { get; set; }
        public int ScoreCategoryId { get; set; }
        public decimal MarksGiven { get; set; }
    }
}
 