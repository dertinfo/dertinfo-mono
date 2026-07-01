using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class TeamAttendance : DatabaseEntity_WithPermissions
    {
        public TeamAttendance()
        {
            CompetitionEntries = new HashSet<CompetitionEntry>();
            Dances = new HashSet<Dance>();
        }

        public int TeamId { get; set; }
        public int RegistrationId { get; set; }
        public string AttendanceNotes { get; set; }

        public virtual ICollection<ActivityTeamAttendance> TeamActivities { get; set; }
        public virtual ICollection<CompetitionEntry> CompetitionEntries { get; set; }
        public virtual ICollection<Dance> Dances { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual Team Team { get; set; }
    }
}
