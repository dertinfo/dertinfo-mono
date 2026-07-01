using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class EventOverviewDO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EventPictureUrl { get; set; }
        public string EventSynopsis { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string LocationTown { get; set; }
        public string LocationPostcode { get; set; }
        public bool IsConfigured { get; set; }
        public bool IsCancelled { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public int RegistrationsCount { get; set; }
        public EventVisibilityType Visibility { get; set; }
        public string SentEmailsBcc { get; set; }
        public int MembersAndGuestsCount { get; set; }
        public int TeamsCount { get; set; }
        public ICollection<EventImage> EventImages { get; set; }
    }
}
