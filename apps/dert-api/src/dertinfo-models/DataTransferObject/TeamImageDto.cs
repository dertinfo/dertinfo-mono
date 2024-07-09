using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class TeamImageDto
    {
        public int TeamImageId { get; set; }
        public int TeamId { get; set; }
        public int ImageId { get; set; }
        public string ImageResourceUri { get; set; }
    }
}
