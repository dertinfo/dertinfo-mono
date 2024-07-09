using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.Paperwork
{
    public class SignInSheetDO
    {
        public Event Event { get; set; }
        public Group Group { get; set; }
        public Registration Registration { get; set; }
        public List<MemberAttendance> MemberAttendances { get; set; }
        public List<TeamAttendance> TeamAttendances { get; set; }
    }
}
