using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class EventCancellationOptionsDO
    {
        public bool SendCommunications { get; set; }

        public List<RegistrationFlowState> CommunicateToStates { get; set; }
    }
}
