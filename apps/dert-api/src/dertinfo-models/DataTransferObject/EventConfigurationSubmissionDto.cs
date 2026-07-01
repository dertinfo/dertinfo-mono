using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventConfigurationSubmissionDto
    {
        public string EventSynopsis { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public int VisibilityType { get; set; }
        public int TemplateType { get; set; }
        public string LocationTown { get; set; }
        public string LocationPostcode { get; set; }
        public bool AgreeToTermsAndConditions { get; set; }
    }
}

