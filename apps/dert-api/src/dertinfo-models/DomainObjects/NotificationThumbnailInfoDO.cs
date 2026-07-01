using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class NotificationThumbnailInfoDO
    {
        public bool HasUnreadMessages { get; set; }
        public NotificationSeverity MaximumMessageSeverity { get; set; }
        public bool HasBlocking { get; set; }
        public int BlockingNotificationLogId { get; set; }
        public DateTime PreviousCheck { get; set; }
        public DateTime LatestCheck { get; set; }
    }
}
