using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class JudgeSlotInformationDto
    {
        public int JudgeSlotId { get; set; }

        public int? JudgeId { get; set; }

        public string JudgeName { get; set; }

        public IList<DanceScorePartDto> ScoreParts { get; set; }
    }
}
