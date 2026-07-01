using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupImageDto
    {
        public int GroupImageId { get; set; }
        public int GroupId { get; set; }
        public int ImageId { get; set; }
        public string ImageResourceUri { get; set; }
    }
}
 