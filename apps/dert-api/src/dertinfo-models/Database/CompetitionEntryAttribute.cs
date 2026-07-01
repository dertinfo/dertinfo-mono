using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class CompetitionEntryAttribute: DatabaseEntity_WithPermissions
    {
        public CompetitionEntryAttribute()
        {
            DertCompetitionEntryAttributeDertCompetitionEntries = new HashSet<DertCompetitionEntryAttributeDertCompetitionEntry>();
        }

        public int CompetitionAppliesToId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }

        public virtual ICollection<DertCompetitionEntryAttributeDertCompetitionEntry> DertCompetitionEntryAttributeDertCompetitionEntries { get; set; }
        public virtual Competition CompetitionAppliesTo { get; set; }
    }
}
