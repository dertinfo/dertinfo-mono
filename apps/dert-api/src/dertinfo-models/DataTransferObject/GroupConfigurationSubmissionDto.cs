using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupConfigurationSubmissionDto
    {
        public string GroupBio { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public int VisibilityType { get; set; }
        public string OriginTown { get; set; }
        public string OriginPostcode { get; set; }
        public bool AgreeToTermsAndConditions { get; set; }
    }
}

