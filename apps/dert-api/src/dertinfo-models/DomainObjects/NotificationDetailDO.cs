using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class NotificationDetailDO
    {
        public int NotificationAudienceLogId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool CanClear { get; set; }
        public bool RequestAcknowledgement { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
