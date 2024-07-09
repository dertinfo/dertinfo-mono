using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class ImageResizeSubmissionDto
    {
        public string OriginalImageUri { get; set; }
        public string RequiredSize { get; set; }

        public bool MaintainScale { get; set; }
    }
}
