using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class TeamAggregateScore : DatabaseEntity
    {
        public int DertTeamId { get; set; }
        public int DertYear { get; set; }
        public bool Organiser { get; set; }
        public decimal AggregateScore { get; set; }

        public virtual Team DertTeam { get; set; }
    }
}
