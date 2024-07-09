using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class GroupMember : DatabaseEntity_WithPermissions
    {
        public GroupMember()
        {
            MemberAttendances = new HashSet<MemberAttendance>();
        }

        public int GroupId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber  { get; set; }
        public string Facebook { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateJoined { get; set; }
        public MemberType MemberType { get; set; }

        public virtual ICollection<MemberAttendance> MemberAttendances { get; set; }
        public virtual Group Group { get; set; }


        /*
         * Futher development here will include items that a third party could add to the group member. 
         * These include the locations where the group member could be selected from. Such as facebook / twitter
         */
    }
}
