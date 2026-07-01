using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Models.Emails
{
    public class EmailBase
    {
        public string[] ToAddresses { get; set; }
        public string[] CcAddresses { get; set; }
        public string[] BccAddresses { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public List<KeyValuePair<string, Byte[]>> Attachments { get; set; }
        public int EmailTemplateId { get; set; }

        public virtual List<KeyValuePair<string, string>> ExtractReplacements()
        {
            var replacements = new List<KeyValuePair<string, string>>();

            return replacements;
        }

        public virtual bool IsTemplateValid(string templateRef)
        {
            return false;
        }

        public string ConvertDate(DateTime? date)
        {
            if (date != null)
            {
                return date.Value.ToString("ddd d MMM yyyy");
            }

            return string.Empty;
        }
    }


    
}
