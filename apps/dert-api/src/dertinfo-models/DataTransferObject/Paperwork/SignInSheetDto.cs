using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Paperwork
{
    public class SignInSheetDto
    {
        public string GroupName { get; set; }
        public string EventName { get; set; }
        public string MemberAttendanceCount { get; set; }
        public string TeamAttendanceCount { get; set; }
        public string GroupMemberPinCode { get; set; }

        public EventDto Event { get; set; }
        public RegistrationDto Registration { get; set; }
        public List<MemberAttendanceDto> MemberAttendances { get; set; }
        public List<TeamAttendanceDto> TeamAttendances { get; set; }
    }
}
