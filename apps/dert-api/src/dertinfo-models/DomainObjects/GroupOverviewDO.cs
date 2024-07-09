using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class GroupOverviewDO
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupBio { get; set; }
        public bool IsConfigured { get; set; }
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
        public GroupVisibilityType Visibility { get; set; }
        public ICollection<GroupImage> GroupImages { get; set; }
    }
}
