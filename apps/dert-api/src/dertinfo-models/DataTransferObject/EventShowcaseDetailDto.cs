using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventShowcaseDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EventPictureUrl { get; set; }
        public string EventSynopsis { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string LocationTown { get; set; }
        public string LocationPostcode { get; set; }
        public bool EventFinished { get; set; }
    }
}
