using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DertInfo.Models.Database
{
    public class NotificationAudienceLog : DatabaseEntity
    {
        /// <summary>
        /// The forign key to the mesage
        /// </summary>
        public int NotificationMessageId { get; set; }

        /// <summary>
        /// The users unique identifier.
        /// </summary>
        public string UserAuth0Identifier { get; set; }


        [NotMapped]
        public bool IsNotifified { get { return this.DateNotifiedOn != null; } }

        /// <summary>
        /// This is the date that the message has been sent to the user. 
        /// It represents the date that there was a "ismessages" tag on the user account
        /// </summary>
        public DateTime? DateNotifiedOn { get; set; }

        [NotMapped]
        public bool IsSeen { get { return this.DateSeenOn != null; } }

        /// <summary>
        /// This is the date that the user has opened the message pane and each of the messages
        /// summaries have been seen. 
        /// </summary>
        public DateTime? DateSeenOn { get; set; }

        [NotMapped]
        public bool IsOpened { get { return this.DateOpenedOn != null; } }

        /// <summary>
        /// This is the date that the user has opened the message pane and each of the messages
        /// summaries have been seen. 
        /// </summary>
        public DateTime? DateOpenedOn { get; set; }

        /// <summary>
        /// The user has interacted with the message and confirmed the message either by clicking ok
        /// or some other action. e.g. a blocking message has been cleared.
        /// </summary>
        [NotMapped]
        public bool IsAcknowledged { get { return this.DateAcknowledgedOn != null; } }

        /// <summary>
        /// The date of Acknowledgement
        /// </summary>
        public DateTime? DateAcknowledgedOn { get; set; }

        /// <summary>
        /// The message has been dismissed no longer has this message displayed in summaries.
        /// </summary>
        [NotMapped]
        public bool IsDismissed { get { return this.DateDismissedOn != null; } }

        /// <summary>
        /// The date time of the clearance
        /// </summary>
        public DateTime? DateDismissedOn { get; set; }

        /// <summary>
        /// The message has been cleared so that the user will no longer has this message displayed anywhere.
        /// </summary>
        [NotMapped]
        public bool IsCleared { get { return this.DateClearedOn != null; } }

        /// <summary>
        /// The date time of the clearance
        /// </summary>
        public DateTime? DateClearedOn { get; set; }

        public virtual NotificationMessage NotificationMessage { get; set; }

        public bool AllowDismiss() {

            var requiresOpening = this.NotificationMessage.RequiresOpening;
            var requiresAcknowledgement = this.NotificationMessage.RequiresAcknowledgement;
            var hasBeenDeleted = this.NotificationMessage.IsDeleted;

            // Doesn't require opening
            if (!requiresOpening || hasBeenDeleted) return true;
            // Does require opening but not acknowledgement
            if (requiresOpening && !requiresAcknowledgement) return this.DateOpenedOn != null;
            // Does require opening and requires acknowledgement
            if (requiresOpening && requiresAcknowledgement) return this.DateAcknowledgedOn != null;

            throw new Exception("Identifying if the notifcation is dismissable logic failed to identify");
        }

        public bool AllowClear()
        {

            var requiresOpening = this.NotificationMessage.RequiresOpening;
            var requiresAcknowledgement = this.NotificationMessage.RequiresAcknowledgement;

            // Doesn't require opening
            if (!requiresOpening) return true;
            // Does require opening but not acknowledgement
            if (requiresOpening && !requiresAcknowledgement) return this.DateOpenedOn != null;
            // Does require opening and requires acknowledgement
            if (requiresOpening && requiresAcknowledgement) return this.DateAcknowledgedOn != null;

            throw new Exception("Identifying if the notifcation is clearable logic failed to identify");
        }
    }
}
