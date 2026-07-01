using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class UserGdprInformationSubmissionDto
    {
        public bool GdprConsentGained { get; set; }
        public DateTime? GdprConsentGainedDate { get; set; }
    }
}
