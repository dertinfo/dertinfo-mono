using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class MemberAttendance : DatabaseEntity_WithPermissions
    {
        public int RegistrationId { get; set; }
        public int GroupMemberId { get; set; }
        public int? AttendanceClassificationId { get; set; }

        public virtual ICollection<ActivityMemberAttendance> MemberActivities { get; set; }
        public virtual AttendanceClassification AttendanceClassification { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual Registration Registration { get; set; }
    }
}
