using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class CompetitionTeamEntryDO
    {
        public CompetitionEntry CompetitionEntry { get; set; }
        public Team Team { get; set; }
        public IEnumerable<CompetitionEntryAttribute> EntryAttributes { get; set; }
        public TeamAttendance TeamAttendance { get; set; }
        public bool HasBeenAddedToCompetition { get; set; }
    }
}
