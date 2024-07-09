using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupOverviewDto: GroupDto
    {
        public string GroupEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactName { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string OriginTown { get; set; }
        public string OriginPostcode { get; set; }
        public int TeamsCount { get; set; }
        public int RegistrationsCount { get; set; }
        public int MembersCount { get; set; }
        public int UnpaidInvoicesCount { get; set; }
        public string GroupMemberJoiningPinCode { get; set; }
        public int Visibility { get; set; }
    }
}
