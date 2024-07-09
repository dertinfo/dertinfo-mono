using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class GdprNotification : EmailBase
    {
        public GdprNotification() {
            
        }

        public string UserDisplayedName { get; set; }

        public string SiteLink { get; set; }

        public string Year { get; set; }

        public override bool IsTemplateValid(string templateRef)
        {
            return templateRef.Trim().ToUpper() == "GDPR_NOTIFICATION";
        }

        public override List<KeyValuePair<string, string>> ExtractReplacements()
        {
            var replacements = new List<KeyValuePair<string, string>>();

            replacements.Add(new KeyValuePair<string, string>("USERDISPLAYEDNAME", this.UserDisplayedName));
            replacements.Add(new KeyValuePair<string, string>("YEAR", this.Year));
            replacements.Add(new KeyValuePair<string, string>("SITEURL", this.SiteLink));

            return replacements;
        }

    }
}
