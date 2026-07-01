using DertInfo.Models.DataTransferObject.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventCompetitionDto
    {
        public int CompetitionId { get; set; }

        public string CompetitionName { get; set; }

        public StatusBlockDto Status { get; set; }

        public StatusBlockDto Judges { get; set; }

        public StatusBlockDto Venues { get; set; }

        public StatusBlockDto Entrants { get; set; }
    }
}
