using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupPictureUrl { get; set; }
        public string GroupBio { get; set; }
        public GroupAccessContext UserAccessContext { get; set; }
        public bool IsConfigured { get; set; }
    }
}
