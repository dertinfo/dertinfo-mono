using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class RegistrationTeamAttendanceSubmissionDto
    {
        public int TeamAttendanceId { get; set; }
        public int TeamId { get; set; }
        public GroupTeamSubmissionDto GroupTeamSubmission { get; set; }
    }
}
