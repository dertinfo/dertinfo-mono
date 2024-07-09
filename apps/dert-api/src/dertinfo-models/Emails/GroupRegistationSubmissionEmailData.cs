using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class GroupRegistationSubmissionEmailData : EmailBase
    {
        public GroupRegistationSubmissionEmailData() {
            this.IndividualAttendanceLineItems = new List<IndividualAttendanceLineItem>();
            this.TeamAttendanceLineItems = new List<TeamAttendanceLineItem>();
        }

        public string GroupName { get; set; }

        public string ContactName { get; set; }

        public string ContactNumber { get; set; }

        public string EventName { get; set; }

        public DateTime? EventRegistrationCloseDate { get; set; }

        public List<IndividualAttendanceLineItem> IndividualAttendanceLineItems;

        public List<TeamAttendanceLineItem> TeamAttendanceLineItems;

        public string SiteLink { get; set; }

        public string Year { get; set; }

        public override bool IsTemplateValid(string templateRef)
        {
            return templateRef.Trim().ToUpper() == "REGISTRATION_SUBMIT";
        }

        public override List<KeyValuePair<string, string>> ExtractReplacements()
        {
            var replacements = new List<KeyValuePair<string, string>>();

            replacements.Add(new KeyValuePair<string, string>("FROMEMAIL", this.FromAddress));
            replacements.Add(new KeyValuePair<string, string>("GROUPNAME", this.GroupName));
            replacements.Add(new KeyValuePair<string, string>("PRIMARYCONTACTNAME", this.ContactName));
            replacements.Add(new KeyValuePair<string, string>("PRIMARYCONTACTNUMBER", this.ContactNumber));
            replacements.Add(new KeyValuePair<string, string>("EVENTNAME", this.EventName));
            replacements.Add(new KeyValuePair<string, string>("EVENTREGISTRATIONCLOSEDATE", this.ConvertDate(this.EventRegistrationCloseDate)));
            replacements.Add(new KeyValuePair<string, string>("LISTATTENDINGMEMBERS", this.BuildMembersList()));
            replacements.Add(new KeyValuePair<string, string>("LISTATTENDINGTEAMS", this.BuildTeamsList()));
            replacements.Add(new KeyValuePair<string, string>("NOOFTEAMS", this.TeamAttendanceLineItems.Count.ToString()));
            replacements.Add(new KeyValuePair<string, string>("NOOFMEMBERS", this.IndividualAttendanceLineItems.Count.ToString()));
            replacements.Add(new KeyValuePair<string, string>("YEAR", this.Year));
            replacements.Add(new KeyValuePair<string, string>("SITEURL", this.SiteLink));
            return replacements;
        }

        private string BuildMembersList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var individualAttendanceLineItem in this.IndividualAttendanceLineItems)
            {
                sb.Append("<li>");
                sb.Append(individualAttendanceLineItem.FullName + " - " + BuildActivitiesMarkup(individualAttendanceLineItem.Activities));
                sb.Append("</li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        private string BuildTeamsList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var teamAttendanceLineItem in this.TeamAttendanceLineItems)
            {
                sb.Append("<li>");
                sb.Append(teamAttendanceLineItem.TeamName + " - " + BuildActivitiesMarkup(teamAttendanceLineItem.Activities));
                sb.Append("</li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        private string BuildActivitiesMarkup(List<ActivityLineItem> activityLineItems)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            foreach (var activity in activityLineItems)
            {
                sb.Append(activity.ActivityName);
                if (activity != activityLineItems.ToArray()[activityLineItems.Count - 1])
                {
                    sb.Append(" / ");
                }
            }
            sb.Append(".");

            return sb.ToString();
        }
    }
}
