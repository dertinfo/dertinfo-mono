using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DertInfo.Models.Database
{
    public class NotificationLastCheck: DatabaseEntity
    {
        /// <summary>
        /// The users Auth0 unique identifier the primary key for this table.
        /// </summary>
        public string UserAuth0Identifier { get; set; }

        /// <summary>
        /// This is the date that the user last checked for thier messages. This check happens when the user first accesses the dashboard. 
        /// After the first check we should also check regularily but this will be functionality on the client.
        /// </summary>
        public DateTime LastCheckPerformedAt { get; set; }

        /// <summary>
        /// Flag to identify that at the time of checking if the user has unread messages. If they do then this should be carried forward.
        /// If they do not then this will be based on the results of the check for new ones. 
        /// </summary>
        /// <remarks>
        /// The accuracy of this field MUST be maintained when messages are opened. 
        /// </remarks>
        public bool HasUnreadMessages { get; set; }


        /// <summary>
        /// This flag is to identify at the time of checking what the maximum priority is of any of the users unread messages. The highest priority should always be caried forward
        /// If they do not then this will be based on the results of the check for new ones. 
        /// </summary>
        /// <remarks>
        /// The accuracy of this field MUST be maintained when messages are opened. 
        /// </remarks>
        public NotificationSeverity MaximumMessageSeverity { get; set; }

        /// <summary>
        /// This flag is to identify if any of the new messages for the user are blocking and require the user to acknowledge before continuing.
        /// </summary>
        /// <remarks>
        /// We do not need to store this value as it's immediately used when the check is performed. As soon as a message becomes blocking then we 
        /// stop the user continuing to use the site until they have acknowleged it.
        /// </remarks>
        [NotMapped]
        public bool HasBlocking { get; set; }

        /// <summary>
        /// This is the id of the blocking notification message
        /// </summary>
        /// <remarks>
        /// Note that the system will only handle 1 blocking message at a time. Therefore the most recent blocking message is used.
        /// </remarks>
        [NotMapped]
        public int BlockingNotificationId { get; set; }

        /// <summary>
        /// This the Id of the Audience Log entry associated with the blocking notificationId for the user.
        /// </summary>
        [NotMapped]
        public int BlockingNotificationLogId { get; set; }
    }
}
