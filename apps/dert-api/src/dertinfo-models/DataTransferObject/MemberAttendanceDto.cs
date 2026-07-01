using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    /// <summary>
    /// This model describes the member attendance. It is used in 2 scenarios:
    /// - As attachment to get a group member detail
    /// - As attachment to registration (does not invlude Event Name and Event Images)
    /// </summary>
    public class MemberAttendanceDto
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public int GroupMemberId { get; set; }
        public int AttendanceClassificationId { get; set; } // todo - remove as we are moving to multi-ticket
        public string GroupMemberName { get; set; }
        public int GroupMemberType { get; set; }

        public string EventName { get; set; }
        public string EventPictureUrl { get; set; }
        public string AttendanceClassificationName { get; set; } // todo - remove - remove as we are moving to multi-ticket
        public string AttendanceClassificationPrice { get; set; } // todo - remove - remove as we are moving to multi-ticket

        public List<ActivityMemberAttendanceDto> AttendanceActivities { get; set; }
}
}
