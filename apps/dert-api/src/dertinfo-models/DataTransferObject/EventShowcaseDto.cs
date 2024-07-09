using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventShowcaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EventPictureUrl { get; set; }
        public DateTime? EventStartDate { get; set; }
        public bool EventFinished { get; set; }
    }
}
