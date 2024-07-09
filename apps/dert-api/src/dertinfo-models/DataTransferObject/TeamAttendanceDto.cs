using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class TeamAttendanceDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int RegistrationId { get; set; }
        public string GroupTeamName { get; set; }
        public string EventName { get; set; }
        public string EventPictureUrl { get; set; }

        public List<ActivityTeamAttendanceDto> AttendanceActivities { get; set; }
    }
}
