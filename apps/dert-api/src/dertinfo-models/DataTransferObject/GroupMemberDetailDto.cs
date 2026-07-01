using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupMemberDetailDto
    {
        public int GroupMemberId { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string Facebook { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateJoined { get; set; }
        public int? MemberType { get; set; }

        public virtual ICollection<MemberAttendanceDto> MemberAttendances { get; set; }

        /*
         * Possible additions here are:
         * - messages they have sent.
         * - payments they have made.
         * - ...
         * 
         */
    }
}
