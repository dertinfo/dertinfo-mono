using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class TeamShowcaseDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamPictureUrl { get; set; }
        public string TeamBio { get; set; }
        public List<EventShowcaseDto> AttendedEvents { get; set; }
    }
}
