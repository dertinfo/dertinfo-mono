using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    /// <summary>
    /// This is the object returned to the interface to fulfil the overview view on a registration viewd from the event perspective
    /// </summary>
    public class EventRegistrationOverviewDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int GroupId { get; set; }
        public string EventName { get; set; }
        public string GroupName { get; set; }
        public string EventPictureUrl { get; set; }
        public string GroupPictureUrl { get; set; }
        public int TeamAttendancesCount { get; set; }
        public int MemberAttendancesCount { get; set; }
        public int GuestAttendancesCount { get; set; }
        public decimal InvoicedTotal { get; set; }
        public decimal CurrentTotal { get; set; }
        public int FlowState { get; set; }
        public bool IsGroupDeleted { get; set; }
    }
}
