using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.Structures
{
    public class StatusBlock
    {
        public Flag Flag { get; set; }

        public string Title { get; set; }

        public string SubText { get; set; }

        public IEnumerable<string> DetailItems { get; set; }
    }
}
