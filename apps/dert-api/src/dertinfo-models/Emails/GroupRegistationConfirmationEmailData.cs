using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class GroupRegistationConfirmationEmailData : EmailBase
    {
        public GroupRegistationConfirmationEmailData()
        {
            this.IndividualAttendanceLineItems = new List<IndividualAttendanceLineItem>();
            this.TeamAttendanceLineItems = new List<TeamAttendanceLineItem>();
        }

        public string GroupName { get; set; }

        public string ContactName { get; set; }

        public string ContactNumber { get; set; }

        public string EventName { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public DateTime? EventRegistrationCloseDate { get; set; }

        public DateTime? PaymentDueDate { get; set; }

        public List<IndividualAttendanceLineItem> IndividualAttendanceLineItems;

        public List<TeamAttendanceLineItem> TeamAttendanceLineItems;

        public string SiteLink { get; set; }

        public string Year { get; set; }

        public virtual bool isTemplateValid(string templateRef)
        {
            return false;
        }

        public override bool IsTemplateValid(string templateRef)
        {
            return templateRef.Trim().ToUpper() == "REGISTRATION_CONFIRM";
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
            replacements.Add(new KeyValuePair<string, string>("INVOICEPAYMENTDUEDATE", this.ConvertDate(this.PaymentDueDate)));
            replacements.Add(new KeyValuePair<string, string>("INVOICEBREAKDOWN", this.BuildInvoiceBreakdown()));
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

        public string BuildInvoiceBreakdown()
        {
            //Build the lines from the team attendance and member attendance

            decimal runningInvoiceTotal = 0;

            //Build the Table Markup
            StringBuilder sb = new StringBuilder();
            sb.Append("<table style='text-align:left;' width='100%'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th>Line</th>");
            sb.Append("<th>Qty</th>");
            sb.Append("<th style='text-align:right;'>SubTotal</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");

            this.CollateIndividualLines(this.IndividualAttendanceLineItems, out Dictionary<string, KeyValuePair<int, decimal>> collatedIndividualLineItems, out Dictionary<string, decimal> individualClassIficationUnitPrice, out decimal individualLinesTotal);
            this.CollateTeamLines(this.TeamAttendanceLineItems, out Dictionary<string, KeyValuePair<int, decimal>> collatedTeamActivityLineItems, out Dictionary<string, decimal> teamActivityUnitPrice, out decimal teamLinesTotal);

            runningInvoiceTotal = individualLinesTotal + teamLinesTotal;

            foreach (var key in collatedIndividualLineItems.Keys) {
                var kvpCollatedLineItem = collatedIndividualLineItems[key];
                var lineItemPrice = individualClassIficationUnitPrice[key];
                sb.Append("<tr>");
                sb.Append("<td>Individual: " + key + " @ £" + lineItemPrice.ToString("f") + " </td>");
                sb.Append("<td>" + kvpCollatedLineItem.Key.ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>£" + kvpCollatedLineItem.Value.ToString("f") + "</td>");
                sb.Append("</tr>");
            }

            foreach (var key in collatedTeamActivityLineItems.Keys)
            {
                var kvpCollatedLineItem = collatedTeamActivityLineItems[key];
                var lineItemPrice = teamActivityUnitPrice[key];
                sb.Append("<tr>");
                sb.Append("<td>Team: " + key + " @ £" + lineItemPrice.ToString("f") + " </td>");
                sb.Append("<td>" + kvpCollatedLineItem.Key.ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>£" + kvpCollatedLineItem.Value.ToString("f") + "</td>");
                sb.Append("</tr>");
            }

            sb.Append("<tr>");
            sb.Append("<td></td>");
            sb.Append("<td><strong>INVOICE TOTAL</strong></td>");
            sb.Append("<td style='text-align:right;'><strong>£" + runningInvoiceTotal.ToString("f") + "</strong></td>");
            sb.Append("</tr>");

            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }

        public void CollateIndividualLines(List<IndividualAttendanceLineItem> individualAttendanceLineItems, out Dictionary<string, KeyValuePair<int, decimal>> collatedIndividualLineItems, out Dictionary<string, decimal> individualActivityUnitPrice, out decimal linesTotal)
        {
            linesTotal = 0;

            collatedIndividualLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            individualActivityUnitPrice = new Dictionary<string, decimal>();

            foreach (var individualLineItem in this.IndividualAttendanceLineItems)
            {
                foreach (var activity in individualLineItem.Activities)
                {
                    var activityPrice = activity.ActivityPrice;

                    if (collatedIndividualLineItems.ContainsKey(activity.ActivityName))
                    {
                        var dictionaryEntry = collatedIndividualLineItems[activity.ActivityName];
                        var instanceCount = dictionaryEntry.Key;
                        var instanceTotal = dictionaryEntry.Value;

                        collatedIndividualLineItems[activity.ActivityName] = new KeyValuePair<int, decimal>(++instanceCount, instanceTotal + activityPrice);

                        linesTotal += activityPrice;
                    }
                    else
                    {
                        individualActivityUnitPrice[activity.ActivityName] = activityPrice;
                        collatedIndividualLineItems[activity.ActivityName] = new KeyValuePair<int, decimal>(1, activityPrice);

                        linesTotal += activityPrice;
                    }
                }
            }
        }

        public void CollateTeamLines(List<TeamAttendanceLineItem> individualAttendanceLineItems, out Dictionary<string, KeyValuePair<int, decimal>> collatedTeamActivityLineItems, out Dictionary<string, decimal> teamActivityUnitPrice, out decimal linesTotal)
        {
            linesTotal = 0; 

            collatedTeamActivityLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            teamActivityUnitPrice = new Dictionary<string, decimal>();

            foreach (var teamLineItem in this.TeamAttendanceLineItems)
            {
                foreach (var activity in teamLineItem.Activities)
                {
                    var activityPrice = activity.ActivityPrice;

                    if (collatedTeamActivityLineItems.ContainsKey(activity.ActivityName))
                    {
                        var dictionaryEntry = collatedTeamActivityLineItems[activity.ActivityName];
                        var instanceCount = dictionaryEntry.Key;
                        var instanceTotal = dictionaryEntry.Value;

                        collatedTeamActivityLineItems[activity.ActivityName] = new KeyValuePair<int, decimal>(++instanceCount, instanceTotal + activityPrice);

                        linesTotal += activityPrice;
                    }
                    else
                    {
                        teamActivityUnitPrice[activity.ActivityName] = activityPrice;
                        collatedTeamActivityLineItems[activity.ActivityName] = new KeyValuePair<int, decimal>(1, activityPrice);

                        linesTotal += activityPrice;
                    }
                }
            }
        }
    }
}

