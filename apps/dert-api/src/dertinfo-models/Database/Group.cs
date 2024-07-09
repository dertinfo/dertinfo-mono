using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Group : DatabaseEntity_WithPermissions
    {
        public Group()
        {
            GroupImages = new HashSet<GroupImage>();
            GroupMembers = new HashSet<GroupMember>();
            Registrations = new HashSet<Registration>();
            Teams = new HashSet<Team>();
        }

        public string GroupName { get; set; }
        public string GroupBio { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string GroupImageUrl { get; set; }
        public string GroupMemberJoiningPinCode { get; set; }
        public string OriginTown { get; set; }
        public string OriginPostcode { get; set; }
        public GroupVisibilityType GroupVisibilityType { get; set; }
        public bool TermsAndConditionsAgreed { get; set; }
        public string TermsAndConditionsAgreedBy { get; set; }

        public bool IsConfigured { get; set; }

        public virtual ICollection<GroupImage> GroupImages { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}
