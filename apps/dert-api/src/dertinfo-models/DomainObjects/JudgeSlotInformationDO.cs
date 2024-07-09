using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class JudgeSlotInformationDO
    {
        public int JudgeSlotId { get; set; }

        public int? JudgeId { get; set; }

        public string JudgeName { get; set; }

        public IList<DanceScorePartDO> ScoreParts { get; set; }


    }
}
