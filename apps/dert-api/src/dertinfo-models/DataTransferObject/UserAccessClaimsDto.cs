using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class UserAccessClaimsDto
    {
        public string Auth0UserId { get; set; }
        public string[] GroupPermissions { get; set; }
        public string[] EventPermissions { get; set; }
        public string[] VenuePermissions { get; set; }
    }
}
