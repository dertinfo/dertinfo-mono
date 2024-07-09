using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Emails
{
    public class EmailTeamAttendanceLineItemDto
    {
        public string TeamName { get; set; }

        public List<EmailActivityLineItemDto> Activities { get; set; }
    }
}
