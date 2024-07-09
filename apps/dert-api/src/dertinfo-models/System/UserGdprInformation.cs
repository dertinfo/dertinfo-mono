using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System
{
    public class UserGdprInformation
    {
        public string Auth0UserId { get; set; }
        public bool GdprConsentGained { get; set; } = false;
    }
}
