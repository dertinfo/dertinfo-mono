using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class AttendanceClassification : DatabaseEntity_WithPermissions
    {
        public AttendanceClassification()
        {
            MemberAttendances = new HashSet<MemberAttendance>();
        }

        public int EventId { get; set; }
        public string ClassificationName { get; set; }
        public decimal ClassificationPrice { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<MemberAttendance> MemberAttendances { get; set; }
        public virtual Event Event { get; set; }
    }
}
