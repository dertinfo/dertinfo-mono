namespace DertInfo.Models.Database
{
    public class ActivityMemberAttendance : DatabaseEntity
    {
        public ActivityMemberAttendance()
        {
        }

        public int MemberAttendanceId { get; set; }
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }
        public virtual MemberAttendance MemberAttendance { get; set; }
    }
}