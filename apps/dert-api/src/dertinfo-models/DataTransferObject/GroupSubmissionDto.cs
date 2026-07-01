using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupSubmissionDto
    {
        public string GroupName { get; set; }
        public string GroupBio { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactNumber { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string Base64StringImage { get; set; }
        public string UploadImageExtension { get; set; }
        public string OriginTown { get; set; }
        public string OriginPostcode { get; set; }
    }
}
