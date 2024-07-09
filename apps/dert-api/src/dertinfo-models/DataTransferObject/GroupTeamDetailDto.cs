using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupTeamDetailDto
    {
        public int TeamId { get; set; }
        public int GroupId { get; set; }
        public string TeamName { get; set; }
        public string TeamBio { get; set; }
        public string TeamPictureUrl { get; set; }
        public bool ShowShowcase { get; set; }

        public virtual ICollection<TeamAttendanceDto> TeamAttendances { get; set; }
    }
}
