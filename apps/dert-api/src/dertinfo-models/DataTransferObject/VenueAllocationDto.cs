using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class VenueAllocationDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool JudgesAllocated { get; set; }

        public IEnumerable<JudgeSlotDto> JudgeSlots { get; set; }
    }
}
