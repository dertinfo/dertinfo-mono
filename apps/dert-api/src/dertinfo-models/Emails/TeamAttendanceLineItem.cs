using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class TeamAttendanceLineItem
    {
        public TeamAttendanceLineItem() {
            this.Activities = new List<ActivityLineItem>();
        }
        public string TeamName { get; set; }

        public List<ActivityLineItem> Activities { get; set; }

    }
}
