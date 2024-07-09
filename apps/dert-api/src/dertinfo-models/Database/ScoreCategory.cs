using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class ScoreCategory : DatabaseEntity_WithPermissions
    {
        public ScoreCategory()
        {
            DanceScores = new HashSet<DanceScore>();
            ScoreSetScoreCategories = new HashSet<ScoreSetScoreCategory>();
        }

        public int CompetitionAppliesToId { get; set; }
        public int MaxMarks { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool InScoreSet1 { get; set; }
        public bool InScoreSet2 { get; set; }

        public virtual ICollection<DanceScore> DanceScores { get; set; }
        public virtual ICollection<ScoreSetScoreCategory> ScoreSetScoreCategories { get; set; }
        public virtual Competition CompetitionAppliesTo { get; set; }
    }
}
