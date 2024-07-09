using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class DanceScorePartDO
    {
        public int DanceScorePartId { get; set; }
        public int DanceScoreId { get; set; }
        public int JudgeSlotId { get; set; }
        public int? JudgeId { get; set; }
        public decimal? ScoreGiven { get; set; }

        // Read Only
        public string ScoreCategoryTag { get; set; }

        // Read Only
        public string ScoreCategoryName { get; set; }

        // Read Only
        public string JudgeName { get; set; }

        public int SortOrder { get; set; }

        public bool IsPartOfScoreSet { get; set; }

    }
}
