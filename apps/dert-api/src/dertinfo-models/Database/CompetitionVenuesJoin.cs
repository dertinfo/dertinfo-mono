using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class CompetitionVenuesJoin : DatabaseJoin
    {
        public int CompetitionId { get; set; }
        public int VenueId { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual Venue Venue { get; set; }
    }
}
