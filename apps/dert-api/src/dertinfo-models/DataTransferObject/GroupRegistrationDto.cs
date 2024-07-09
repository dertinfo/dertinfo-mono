using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupRegistrationDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int GroupId { get; set; }
        public string EventName { get; set; } 
        public string GroupName { get; set; }
        public string EventPictureUrl { get; set; }
        public string GroupPictureUrl { get; set; }
        public int TeamAttendancesCount { get; set; }
        public int GuestAttendancesCount { get; set; }
        public int MemberAttendancesCount { get; set; }
        public int FlowState { get; set; }
    }
}
