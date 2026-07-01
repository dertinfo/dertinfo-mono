using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Emails
{
    public class EmailRegistrationConfirmationDataDto: EmailBaseDto
    {
        public EmailRegistrationConfirmationDataDto()
        {
        }

        public string GroupName { get; set; }

        public string ContactName { get; set; }

        public string ContactNumber { get; set; }

        public string EventName { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public DateTime EventRegistrationCloseDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public List<EmailIndividualAttendanceLineItemDto> IndividualAttendanceLineItems;

        public List<EmailTeamAttendanceLineItemDto> TeamAttendanceLineItems;

        public string SiteLink { get; set; }

        public string Year { get; set; }
    }
}
