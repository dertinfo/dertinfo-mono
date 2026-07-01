using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class EventImageSubmissionDto
    {
        public int EventId { get; set; }
        public string Base64StringImage { get; set; }
        public string UploadImageExtension { get; set; }
    }
}
 