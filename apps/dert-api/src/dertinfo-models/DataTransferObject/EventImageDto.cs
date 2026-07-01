using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class EventImageDto
    {
        public int EventImageId { get; set; }
        public int EventId { get; set; }
        public int ImageId { get; set; }
        public string ImageResourceUri { get; set; }
    }
}
 