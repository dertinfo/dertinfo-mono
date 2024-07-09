using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventSubmissionDto
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventSynopsis { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }

        public string TemplateSelection { get; set; }
        public string Base64StringImage { get; set; }
        public string UploadImageExtension { get; set; }
    }
}
