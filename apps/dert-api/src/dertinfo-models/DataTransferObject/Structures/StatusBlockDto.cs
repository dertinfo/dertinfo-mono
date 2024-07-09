using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Structures
{
    public class StatusBlockDto
    {
        public Flag Flag { get; set; }

        public string FlagText { get { return this.Flag.ToString(); } }

        public string Title { get; set; }

        public string SubText { get; set; }

        public IEnumerable<string> DetailItems { get; set; }
    }
}
