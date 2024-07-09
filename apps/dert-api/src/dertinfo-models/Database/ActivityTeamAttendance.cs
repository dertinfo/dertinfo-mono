using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public class ActivityTeamAttendance: DatabaseEntity
    {
        public ActivityTeamAttendance()
        {
        }

        public int TeamAttendanceId { get; set; }
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }
        public virtual TeamAttendance TeamAttendance { get; set; }
    }
}