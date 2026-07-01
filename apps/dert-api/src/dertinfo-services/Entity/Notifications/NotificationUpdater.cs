using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{
    public interface INotificationUpdater
    {
        Task<bool> DismissNotificationByUser(int id);
        Task<bool> AcknowledgeNotification(int id);
        Task<bool> ClearNotification(int id);
    }

    /// <summary>
    /// Create a new message on the system. This is simply for admin users only to create system messages. 
    /// </summary>
    public class NotificationUpdater : INotificationUpdater
    {
        INotificationAudienceLogRepository _notificationAudienceLogRepository;

        public NotificationUpdater(
            INotificationAudienceLogRepository notificationAudienceLogRepository
            )
        {

            _notificationAudienceLogRepository = notificationAudienceLogRepository;
        }

        public async Task<bool> DismissNotificationByUser(int id)
        {
            var notificationLogEntry = await this._notificationAudienceLogRepository.GetDetailForUser(id);

            if (!notificationLogEntry.AllowDismiss()) throw new Exception("Notification is Not Dismissable");

            return await this._notificationAudienceLogRepository.Dismiss(id);

        }

        public async Task<bool> AcknowledgeNotification(int id)
        {
            return await this._notificationAudienceLogRepository.Acknowledge(id);
        }

        public async Task<bool> ClearNotification(int id)
        {
            var notificationLogEntry = await this._notificationAudienceLogRepository.GetDetailForUser(id);

            if (!notificationLogEntry.AllowClear()) throw new Exception("Notification is Not Clearable");

            return await this._notificationAudienceLogRepository.Clear(id);
        }
    }
}
