using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System
{
    public class UserAppMetaData
    {
        public string[] groupadmin { get; set; }
        public string[] eventadmin { get; set; }
        public string[] venueadmin { get; set; }
        public string[] groupmember { get; set; }
        public bool isSuperAdmin { get; set; }
        public bool gdprConsentGained { get; set; }
        public DateTime? gdprConsentGainedDate { get; set; }
    }

}
