using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class JudgeSlot : DatabaseEntity_WithPermissions
    {
        public string Name { get; set; }
        public int VenueId { get; set; }
        public int CompetitionId { get; set; }
        public int MarkingSet { get; set; }
        public int? JudgeId { get; set; }
        public int? ScoreSetId { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual Judge Judge { get; set; }
        public virtual ScoreSet ScoreSet { get; set; }
        public virtual Venue Venue { get; set; }

        public virtual ICollection<DanceScorePart> DanceScoreParts { get; set; }
    }
}
