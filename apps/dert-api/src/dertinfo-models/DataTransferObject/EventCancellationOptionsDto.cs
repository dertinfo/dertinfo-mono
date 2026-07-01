using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventCancellationOptionsDto
    {
        public bool SendCommunications { get; set; }
        public bool InformNewRegistrations { get; set; }
        public bool InformSubmittedRegistrations { get; set; }
        public bool InformConfirmedRegistrations { get; set; }
    }
}
