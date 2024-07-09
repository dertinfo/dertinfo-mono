using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.CrossCutting.Auth
{
    public interface IDertInfoUser
    {
        string AuthId { get; set; }
        UserType UserType { get; set; }

        List<string> ClaimsCompetitionAdmin { get; set; }
        List<string> ClaimsEventAdmin { get; set; }
        List<string> ClaimsGroupAdmin { get; set; }
        List<string> ClaimsGroupMember { get; set; }
        List<string> ClaimsVenueAdmin { get; set; }

        bool IsSuperAdmin { get; set; }
    }

    public class DertInfoUser : IDertInfoUser
    {
        public string AuthId { get; set; }

        public List<string> ClaimsCompetitionAdmin { get; set; }
        public List<string> ClaimsEventAdmin { get; set; }
        public List<string> ClaimsGroupAdmin { get; set; }
        public List<string> ClaimsGroupMember { get; set; }
        public List<string> ClaimsVenueAdmin { get; set; }

        public bool IsSuperAdmin { get; set; }

        public UserType UserType { get; set; }
    }

    public enum UserType {
        SuperAdmin,
        EventAdmin,
        VenueAdmin,
        TeamAdmin, 
        TeamMember
    }
}
