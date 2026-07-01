using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public partial class DanceScorePart : DatabaseEntity
    {
        public int JudgeSlotId { get; set; }

        public int DanceScoreId { get; set; }

        public decimal? ScoreGiven { get; set; }

        public virtual JudgeSlot JudgeSlot { get; set; }

        public virtual DanceScore DanceScore { get; set; }
    }
}
