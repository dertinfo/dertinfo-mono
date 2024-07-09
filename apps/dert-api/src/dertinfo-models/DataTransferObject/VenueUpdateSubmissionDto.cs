using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class VenueUpdateSubmissionDto
    {
        public string Name { get; set; }

        public IEnumerable<JudgeSlotJudgeUpdateSubmissionDto> JudgeSlotUpdates { get; set; }
    }
}
