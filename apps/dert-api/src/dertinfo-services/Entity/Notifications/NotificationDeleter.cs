using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{
    public interface INotificationDeleter
    {
        Task<NotificationMessage> SoftDelete(int notificationMessageId);
    }

    /// <summary>
    /// Service to support the soft delete of messages from the system. Note that we cannot retract messages that have already been seen
    /// and not cleared by the user. Without notifying of the replacement of the message. Messages cannot be physically deleted so depending 
    /// on the state of a message for a user we may need to take different action. If a message is to be replaced due to errors in the original
    /// then we would need to appropriately inform all users of the update.
    /// </summary>
    public class NotificationDeleter : INotificationDeleter
    {

        IAuthService _authService;
        INotificationAudienceLogRepository _notificationAudienceLogRepository;
        INotificationLastCheckRepository _notificationLastCheckRepository;
        INotificationMessageRepository _notificationMessageRepository;
        IDertInfoUser _user;

        public NotificationDeleter(
            IAuthService authService,
            INotificationAudienceLogRepository notificationAudienceLogRepository,
            INotificationLastCheckRepository notificationLastCheckRepository,
            INotificationMessageRepository notificationMessageRepository,
            IDertInfoUser user
            )
        {
            _authService = authService;
            _notificationAudienceLogRepository = notificationAudienceLogRepository;
            _notificationLastCheckRepository = notificationLastCheckRepository;
            _notificationMessageRepository = notificationMessageRepository;
            _user = user;
        }

        /// <summary>
        /// Soft Delete the notification.
        /// </summary>
        /// <param name="notificationMessageId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Soft delete must involve:
        /// - marking the notification as deleted. THis will prevent it appearing in the admin interfaces
        /// - iterate the links to the messages in the audience logs and mark each one deleted.
        /// </remarks>
        public async Task<NotificationMessage> SoftDelete(int notificationMessageId)
        {
            // Soft delete the item
            var item = await this._notificationMessageRepository.GetById(notificationMessageId);

            item.IsDeleted = true;

            await this._notificationMessageRepository.Update(item);

            // Now find all Audience logs and set those deleted
            await this._notificationAudienceLogRepository.ApplyDeletedToLogsByNotificationMessageId(notificationMessageId);

            return item;

        }
    }
}
