using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class RegistrationMemberAttendanceSubmissionDto
    {
        public int MemberAttendanceId { get; set; }
        public int GroupMemberId { get; set; }
        public GroupMemberSubmissionDto GroupMemberSubmission  { get; set; }
    }
}
