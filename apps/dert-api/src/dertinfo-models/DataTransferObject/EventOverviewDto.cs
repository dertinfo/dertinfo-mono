using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventOverviewDto : EventDto
    {
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public int RegistrationsCount { get; set; }
        public int Visibility { get; set; }
        public string SentEmailsBcc { get; set; }
        public int MembersAndGuestsCount { get; set; }
        public int TeamsCount { get; set; }
    }
}
