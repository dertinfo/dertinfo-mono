using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class DanceScore : DatabaseEntity
    {
        public int DanceId { get; set; }
        public int ScoreCategoryId { get; set; }
        public decimal MarkGiven { get; set; }
        public string CommentGiven { get; set; }
        public virtual Dance Dance { get; set; }
        public virtual ScoreCategory ScoreCategory { get; set; }
        public virtual ICollection<DanceScorePart> DanceScoreParts { get; set; }
    }
}
