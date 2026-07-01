using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class CompetitionEntry: DatabaseEntity_WithPermissions
    {
        public CompetitionEntry()
        {
            DertCompetitionEntryAttributeDertCompetitionEntries = new HashSet<DertCompetitionEntryAttributeDertCompetitionEntry>();
        }

        public int TeamAttendanceId { get; set; }
        public int CompetitionId { get; set; }
        public int DertYear { get; set; }
        public bool IsDisabled { get; set; }

        public virtual ICollection<DertCompetitionEntryAttributeDertCompetitionEntry> DertCompetitionEntryAttributeDertCompetitionEntries { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual TeamAttendance TeamAttendance { get; set; }
    }
}
