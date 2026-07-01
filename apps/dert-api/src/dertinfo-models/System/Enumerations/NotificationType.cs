using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.System.Enumerations
{
    public enum NotificationType
    {
        /// <summary>
        /// System => All Users
        /// AdministratorNotification - This type is for system notifications. For example that the terms and conditions have been updated. 
        /// </summary>
        AdministratorNotification = 0,
        /// <summary>
        /// System => Specific User
        /// This message type would be for items such as your registration has been confirmed. Created by the system to the user
        /// </summary>
        InformationUpdate = 1, // Not Implemented
        /// <summary>
        /// User => User
        /// This type would be user 
        /// </summary>
        UserMessage = 2, // Not Implemented
    }
}
