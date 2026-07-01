using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventOverviewUpdateDto
    {

        public int EventId { get; set; }
        public string EventName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactName { get; set; }
        public string EventSynopsis { get; set; }
        public string LocationTown { get; set; }
        public string LocationPostcode { get; set; }
        public int Visibility { get; set; }
        public string SentEmailsBcc { get; set; }

    }
}
