using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class EventCancellationEmailData : EmailBase
    {
        public string GroupName { get; set; }

        public string ContactName { get; set; }

        public string EventName { get; set; }

        public override bool IsTemplateValid(string templateRef)
        {
            return templateRef.Trim().ToUpper() == "EVENT_CANCELLATION";
        }

        public override List<KeyValuePair<string, string>> ExtractReplacements()
        {
            var replacements = new List<KeyValuePair<string, string>>();

            replacements.Add(new KeyValuePair<string, string>("FROMEMAIL", this.FromAddress));
            replacements.Add(new KeyValuePair<string, string>("GROUPNAME", this.GroupName));
            replacements.Add(new KeyValuePair<string, string>("PRIMARYCONTACTNAME", this.ContactName));
            replacements.Add(new KeyValuePair<string, string>("EVENTNAME", this.EventName));

            return replacements;
        }
    }
}
