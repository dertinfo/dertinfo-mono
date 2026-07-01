using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ActivityMemberAttendanceDto
    {
        public int Id { get; set; }
        public int MemberAttendanceId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public decimal ActivityPrice { get; set; }
    }
}
