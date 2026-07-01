using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupRegistrationSubmissionDto
    {
        public int EventId { get; set; }
        public bool AgreeToTermsAndConditions { get; set; }
    }
}
