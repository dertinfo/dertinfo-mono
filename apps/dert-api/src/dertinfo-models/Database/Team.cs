using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Team : DatabaseEntity_WithPermissions
    {
        public Team()
        {
            TeamAggregateScores = new HashSet<TeamAggregateScore>();
            TeamAttendances = new HashSet<TeamAttendance>();
            TeamImages = new HashSet<TeamImage>();
        }

        public int GroupId { get; set; }
        public string TeamName { get; set; }
        public string TeamBio { get; set; }
        public bool ShowShowcase { get; set; }

        public virtual ICollection<TeamAggregateScore> TeamAggregateScores { get; set; }
        public virtual ICollection<TeamAttendance> TeamAttendances { get; set; }
        public virtual ICollection<TeamImage> TeamImages { get; set; }
        public virtual Group Group { get; set; }
    }
}
