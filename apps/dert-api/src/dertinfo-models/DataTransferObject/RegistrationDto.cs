using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class RegistrationDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int EventId { get; set; }
        public int FlowState { get; set; }
    }
}
