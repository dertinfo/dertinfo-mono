using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Registration : DatabaseEntity_WithPermissions
    {
        public Registration()
        {
            Invoices = new HashSet<Invoice>();
            MemberAttendances = new HashSet<MemberAttendance>();
            TeamAttendances = new HashSet<TeamAttendance>();
        }

        public int GroupId { get; set; }
        public int EventId { get; set; }
        public int DertYear { get; set; }
        public int EstimateAttending { get; set; }
        public int EstimateAccomodation { get; set; }
        public string SpecialRequirements { get; set; }
        public RegistrationFlowState FlowState { get; set; }
        public bool TermsAndConditionsAgreed { get; set; }
        public string TermsAndConditionsAgreedBy { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<MemberAttendance> MemberAttendances { get; set; }
        public virtual ICollection<TeamAttendance> TeamAttendances { get; set; }
        public virtual Event Event { get; set; }
        public virtual Group Group { get; set; }
    }
}
