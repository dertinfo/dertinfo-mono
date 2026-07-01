using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{
    public interface INotificationService
    {
        Task<ICollection<NotificationSummaryDO>> GetNotificationSummariesForUser();
        Task<NotificationMessage> CreateNotificationMessage(NotificationMessage notificationMessage);
        Task<NotificationThumbnailInfoDO> CheckForNewNotificationsForUser();
        Task<bool> DismissNotificationByUser(int notificationAudienceLogId);
        Task<bool> AcknowledgeNotification(int notificationAudienceLogId);
        Task<bool> ClearNotification(int notificationAudienceLogId);

        Task<NotificationDetailDO> GetNotificationDetailForUser(int notificationAudienceLogId);
        Task<ICollection<NotificationMessage>> GetNotificationListForAdmin();
        Task<NotificationMessage> DeleteNotificationMessage(int notificationMessageId);
    }

    /// <summary>
    /// Gateway Service for the notifications functionality.
    /// </summary>
    public class NotificationService : INotificationService
    {

        INotificationReader _notificationReader;
        INotificationCreator _notificationCreator;
        INotificationDeleter _notificationDeleter;
        INotificationChecker _notificationChecker;
        INotificationUpdater _notificationUpdater;

        public NotificationService(
            INotificationReader notificationReader,
            INotificationCreator notificationCreator,
            INotificationDeleter notificationDeleter,
            INotificationChecker notificationChecker,
            INotificationUpdater notificationUpdater
        )
        {

            _notificationReader = notificationReader;
            _notificationCreator = notificationCreator;
            _notificationDeleter = notificationDeleter;
            _notificationChecker = notificationChecker;
            _notificationUpdater = notificationUpdater;
        }

        
        /// <summary>
        /// This is the called when the client requests the details for the notification panel. 
        /// It results in displaying the summaries to the user
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<NotificationSummaryDO>> GetNotificationSummariesForUser() {

            return await this._notificationReader.GetNotificationSummariesForUser();
        }

        public async Task<NotificationMessage> CreateNotificationMessage(NotificationMessage notificationMessage)
        {
            return await this._notificationCreator.CreateNotification(notificationMessage);
        }

        public async Task<NotificationThumbnailInfoDO> CheckForNewNotificationsForUser()
        {
            return await this._notificationChecker.CheckForNotifications();
        }

        public async Task<bool> DismissNotificationByUser(int notificationAudienceLogId)
        {
            return await this._notificationUpdater.DismissNotificationByUser(notificationAudienceLogId);
        }

        public async Task<bool> AcknowledgeNotification(int notificationAudienceLogId)
        {
            return await this._notificationUpdater.AcknowledgeNotification(notificationAudienceLogId);
        }

        public async Task<bool> ClearNotification(int notificationAudienceLogId)
        {
            return await this._notificationUpdater.ClearNotification(notificationAudienceLogId);
        }

        public async Task<NotificationDetailDO> GetNotificationDetailForUser(int notificationAudienceLogId)
        {
            return await this._notificationReader.GetNotificationDetailForUser(notificationAudienceLogId);
        }

        public async Task<ICollection<NotificationMessage>> GetNotificationListForAdmin()
        {
            return await this._notificationReader.GetNotificationListForAdmin();
        }

        public async Task<NotificationMessage> DeleteNotificationMessage(int notificationMessageId)
        {
            return await this._notificationDeleter.SoftDelete(notificationMessageId);
        }
    }
}
