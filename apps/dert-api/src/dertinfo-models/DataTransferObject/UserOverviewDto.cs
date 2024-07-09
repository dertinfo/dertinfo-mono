using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class UserOverviewDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public bool GdprConsentGained { get; set; }
        public DateTime? GdprConsentGainedDate { get; set; }
    }
}
