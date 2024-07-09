using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Notifications
{
    public class NotificationMessageDto
    {
        public int Id { get; set; }

        /// <summary>
        /// The title of the message
        /// </summary>
        public string MessageHeader { get; set; }

        /// <summary>
        /// The part of the message that shows in the summary.
        /// </summary>
        public string MessageSummary { get; set; }

        /// <summary>
        /// The full body of the message if there is one.
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// Identifies if the message has any details. If the message has details then it can be
        /// further opened from the summary.
        /// </summary>
        public bool HasDetails { get; set; }

        /// <summary>
        /// Identifies if the user can dismiss the message from the summary
        /// </summary>
        public bool IsDismissable { get; set; }

        /// <summary>
        /// This setting identified that the user cannot continue to use the application until this message has been acknowldged.
        /// These messages will appear over the dashboard as the user logs in. 
        /// </summary>
        public bool BlocksUser { get; set; }

        /// <summary>
        /// Traffic light to indicate how severe the message is.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        public NotificationType NotificationType { get; set; }

        /// <summary>
        /// The date the notification message was created
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
