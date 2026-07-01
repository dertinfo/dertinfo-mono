using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Notifications
{
    public class NotificationThumbnailInfoDto
    {

        public bool HasUnreadMessages { get; set; }
        public NotificationSeverity MaximumMessageSeverity { get; set; }
        public bool HasBlocking { get; set; }
        public int BlockingNotificationLogId { get; set; }
    }
}
