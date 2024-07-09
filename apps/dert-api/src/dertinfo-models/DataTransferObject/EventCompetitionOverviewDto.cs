using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventCompetitionsOverviewDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventPictureUrl { get; set; }

        public List<CompetitionOverviewDto> CompetitionOverviews { get; set; }
    }
}
