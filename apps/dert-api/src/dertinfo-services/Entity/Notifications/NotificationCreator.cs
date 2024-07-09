using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{
    public interface INotificationCreator
    {
        Task<NotificationMessage> CreateNotification(NotificationMessage notificationMessage);
    }

    /// <summary>
    /// Create a new message on the system. This is simply for admin users only to create system messages. 
    /// </summary>
    public class NotificationCreator : INotificationCreator
    {
        INotificationMessageRepository _notificationMessageRepository;

        public NotificationCreator(
            INotificationMessageRepository notificationMessageRepository
            )
        {

            _notificationMessageRepository = notificationMessageRepository;
        }

        public async Task<NotificationMessage> CreateNotification(NotificationMessage notificationMessage)
        {
            return await _notificationMessageRepository.Add(notificationMessage);
        }
    }
}
