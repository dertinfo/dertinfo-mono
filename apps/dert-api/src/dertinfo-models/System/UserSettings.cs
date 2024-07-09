using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System
{
    public class UserSettings
    {
        public string Auth0UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public bool GdprConsentGained { get; set; } = false;
        public DateTime? GdprConsentGainedDate { get; set; }
    }
}
