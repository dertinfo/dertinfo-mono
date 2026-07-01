using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class ScoreSetScoreCategory : DatabaseJoin
    {
        public int ScoreSetId { get; set; }
        public int ScoreCategoryId { get; set; }

        public virtual ScoreCategory ScoreCategory { get; set; }
        public virtual ScoreSet ScoreSet { get; set; }
    }
}
