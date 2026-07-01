using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionEntryAttributeDto
    {
        public int Id { get; set; }
        public int CompetitionAppliesToId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        
    }
}
