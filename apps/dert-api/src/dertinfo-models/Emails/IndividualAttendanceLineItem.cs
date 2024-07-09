using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Emails
{
    public class IndividualAttendanceLineItem
    {
        public IndividualAttendanceLineItem()
        {
            Activities = new List<ActivityLineItem>();
        }

        public string FullName { get; set; }

        /*
         * Classification should be removed and the activity should be added such as entry, accomodation, with team and individual prices.
         * With restriction on what activities are available to teams and individuals.
         */
        public string ClasificationName { get; set; }
        public decimal ClasificationPrice { get; set; }
        public List<ActivityLineItem> Activities { get; set; }
    }
}
