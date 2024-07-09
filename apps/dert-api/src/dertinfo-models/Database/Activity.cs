using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class Activity : DatabaseEntity
    {
        public Activity()
        {
            
        }

        public int EventId { get; set; }
        public int AudienceTypeId { get; set; }
        public int? CompetitionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool PriceTBC { get; set; }
        public decimal Price { get; set; }
        public bool SoldOut { get; set; }

        public virtual ICollection<ActivityMemberAttendance> ParticipatingIndividuals { get; set; }
        public virtual ICollection<ActivityTeamAttendance> ParticipatingTeams { get; set; }
        public virtual Event Event { get; set; }

        public virtual Competition Competition { get; set; }
    }
}
