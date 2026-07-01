using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Event : DatabaseEntity_WithPermissions
    {
        public Event()
        {
            AttendanceClassifications = new HashSet<AttendanceClassification>();
            Activities = new HashSet<Activity>();
            Competitions = new HashSet<Competition>();
            EmailTemplates = new HashSet<EmailTemplate>();
            EventImages = new HashSet<EventImage>();
            EventJudges = new HashSet<EventJudge>();
            EventSettings = new HashSet<EventSetting>();
            Registrations = new HashSet<Registration>();
            Venues = new HashSet<Venue>();
        }

        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public string EventSynopsis { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public EventTemplateType EventTemplateType { get; set; }
        public EventVisibilityType EventVisibilityType { get; set; }
        public string LocationTown { get; set; }
        public string LocationPostcode { get; set; }
        public bool IsConfigured { get; set; }
        public bool IsPromoted { get; set; }
        public bool IsCancelled { get; set; }
        public bool TermsAndConditionsAgreed { get; set; }
        public string TermsAndConditionsAgreedBy { get; set; }
        public string SentEmailsBcc { get; set; }

        public virtual ICollection<AttendanceClassification> AttendanceClassifications { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Competition> Competitions { get; set; }
        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; }
        public virtual ICollection<EventImage> EventImages { get; set; }
        public virtual ICollection<EventJudge> EventJudges { get; set; }
        public virtual ICollection<EventSetting> EventSettings { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
        public virtual ICollection<Venue> Venues { get; set; }
    }
}
