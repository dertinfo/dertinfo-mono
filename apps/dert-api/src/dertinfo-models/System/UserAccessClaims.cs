using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System
{
    public class UserAccessClaims
    {
        public string Auth0UserId { get; set; }
        public string[] GroupPermissions { get; set; } = {};
        public string[] EventPermissions { get; set; } = {};
        public string[] VenuePermissions { get; set; } = {};
        public string[] GroupMemberPermissions { get; set; } = {};
    }
}
