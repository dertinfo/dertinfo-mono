using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class DertCompetitionEntryAttributeDertCompetitionEntry : DatabaseJoin
    {
        public int DertCompetitionEntryId { get; set; }
        public int DertCompetitionEntryAttributeId { get; set; }

        public virtual CompetitionEntryAttribute DertCompetitionEntryAttribute { get; set; }
        public virtual CompetitionEntry DertCompetitionEntry { get; set; }
    }
}
