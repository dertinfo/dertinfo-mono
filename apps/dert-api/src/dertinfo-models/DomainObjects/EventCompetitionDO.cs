using DertInfo.Models.DomainObjects.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class EventCompetitionDO
    {
        public int CompetitionId { get; set; }

        public string CompetitionName { get; set; }

        public StatusBlock Status { get; set; }

        public StatusBlock Judges { get; set; }

        public StatusBlock Venues { get; set; }

        public StatusBlock Entrants { get; set; }
    }
}
