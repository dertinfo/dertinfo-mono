using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Notifications
{
    public class NotificationSummaryDto
    {
        public int NotificationAudienceLogId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool CanOpen { get; set; }
        public bool CanDismiss { get; set; }
        public bool HasBeenOpened { get; set; }
        public bool HasBeenSeen { get; set; }
        public bool HasBeenDeleted { get; set; }
        public bool HasBeenAcknowledged { get; set; }
        public bool MustAcknowledge { get; set; }
        public NotificationSeverity Severity { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
