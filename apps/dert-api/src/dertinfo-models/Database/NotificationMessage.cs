using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class NotificationMessage : DatabaseEntity
    {
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
        public bool RequiresOpening { get; set; }

        /// <summary>
        /// Identifies if the user must actively confirm that they agree
        /// </summary>
        public bool RequiresAcknowledgement { get; set; }

        /// <summary>
        /// This setting identified that the user cannot continue to use the application until this message has been acknowldged.
        /// These messages will appear over the dashboard as the user logs in. 
        /// </summary>
        public bool BlocksUser { get; set; }

        /// <summary>
        /// Traffic Light severity on the message as to how much you need the user to read it.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// This determines which icon to show on the client
        /// </summary>
        public NotificationType NotificationType { get; set; }

        public virtual ICollection<NotificationAudienceLog> NotificationAudience { get; set; }
    }
}
