using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupTeamUpdateDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamBio { get; set; }
    }
}
