using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceImageSubmissionDto
    {
        public int DanceId { get; set; }

        public string Base64StringImage { get; set; }
    }
}
 