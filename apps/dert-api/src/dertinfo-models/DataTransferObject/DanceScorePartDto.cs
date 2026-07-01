using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceScorePartDto
    {
        public int DanceScorePartId { get; set; }
        public int DanceScoreId { get; set; }
        public int JudgeSlotId { get; set; }
        public decimal? ScoreGiven { get; set; }
        public string ScoreCategoryTag { get; set; }
        public int SortOrder { get; set; }
        public bool IsPartOfScoreSet { get; set; }
    }
}
