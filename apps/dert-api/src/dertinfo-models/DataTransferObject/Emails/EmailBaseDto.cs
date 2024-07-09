using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Emails
{
    public class EmailBaseDto
    {
        public string[] ToAddresses { get; set; }
        public string[] CcAddresses { get; set; }
        public string[] BccAddresses { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public List<KeyValuePair<string, Byte[]>> Attachments { get; set; }
        public int EmailTemplateId { get; set; }
    }
}
