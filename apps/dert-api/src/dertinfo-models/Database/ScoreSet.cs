using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class ScoreSet : DatabaseEntity_WithPermissions
    {
        public ScoreSet()
        {
            JudgeSlots = new HashSet<JudgeSlot>();
            ScoreSetScoreCategories = new HashSet<ScoreSetScoreCategory>();
        }

        public int CompetitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<JudgeSlot> JudgeSlots { get; set; }
        public virtual ICollection<ScoreSetScoreCategory> ScoreSetScoreCategories { get; set; }
        public virtual Competition Competition { get; set; }
    }
}
