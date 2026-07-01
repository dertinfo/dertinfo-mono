using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupOverviewUpdateDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactName { get; set; }
        public string GroupBio { get; set; }
        public string OriginTown { get; set; }
        public string OriginPostcode { get; set; }
        public int Visibility { get; set; }
    }
}
