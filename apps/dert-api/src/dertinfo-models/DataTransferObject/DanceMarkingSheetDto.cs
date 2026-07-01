using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceMarkingSheetDto
    {
        public int MarkingSheetId { get; set; }
        public int DanceId { get; set; }
        public string ImageResourceUri { get; set; }
    }
}
 