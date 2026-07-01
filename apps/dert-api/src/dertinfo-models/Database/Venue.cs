using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Venue : DatabaseEntity_WithPermissions
    {
        public Venue()
        {
            CompetitionVenuesJoin = new HashSet<CompetitionVenuesJoin>();
            Dances = new HashSet<Dance>();
            JudgeSlots = new HashSet<JudgeSlot>();
        }

        public string Name { get; set; }
        public string JudgeMinderUsername { get; set; }
        public string Auth0Username { get; set; }
        public int EventId { get; set; }

        public virtual ICollection<CompetitionVenuesJoin> CompetitionVenuesJoin { get; set; }
        public virtual ICollection<Dance> Dances { get; set; }
        public virtual ICollection<JudgeSlot> JudgeSlots { get; set; }
        public virtual Event Event { get; set; }
    }
}
