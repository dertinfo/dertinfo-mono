using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Repository;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{
    public interface INotificationReader
    {
        Task<ICollection<NotificationSummaryDO>> GetNotificationSummariesForUser();

        Task<NotificationDetailDO> GetNotificationDetailForUser(int notificationAudienceLogId);
        Task<ICollection<NotificationMessage>> GetNotificationListForAdmin();
    }

    /// <summary>
    /// Service class for users to get the message lists per user and the details of the message is there are any. 
    /// </summary>
    public class NotificationReader : INotificationReader
    {
        INotificationAudienceLogRepository _notificationAudienceLogRepository;
        INotificationMessageRepository _notificationMessageRepository;
        INotificationChecker _notificationChecker;
        IDertInfoUser _user;

        public NotificationReader (
            INotificationAudienceLogRepository notificationAudienceLogRepository,
            INotificationMessageRepository notificationMessageRepository,
            INotificationChecker notificationChecker,
            IDertInfoUser user
            )
        {
            _notificationAudienceLogRepository = notificationAudienceLogRepository;
            _notificationMessageRepository = notificationMessageRepository;
            _notificationChecker = notificationChecker;
            _user = user;
        }

        public async Task<NotificationDetailDO> GetNotificationDetailForUser(int notificationAudienceLogId)
        {
            Ensure.String.IsNotNullOrWhiteSpace(_user.AuthId);

            var detail = await this._notificationAudienceLogRepository.GetDetailForUser(notificationAudienceLogId);

            await this._notificationAudienceLogRepository.MarkAsOpened(notificationAudienceLogId);

            // From all summaries we know that those that must be opened are still valid for the check.
            var summaries = await this.GetSummariesForUser(_user.AuthId);
            await this.UpdateCheckInformationForUser(summaries);

            return new NotificationDetailDO()
            {
                NotificationAudienceLogId = detail.Id,
                Title = detail.NotificationMessage.MessageHeader,
                Body = detail.NotificationMessage.MessageBody,
                IsRead = detail.DateOpenedOn != null,
                CanClear = !detail.NotificationMessage.RequiresAcknowledgement || detail.IsAcknowledged,
                RequestAcknowledgement = detail.NotificationMessage.RequiresAcknowledgement ? !detail.IsAcknowledged : false,
                DateCreated = detail.DateCreated
            };
        }

        public async Task<ICollection<NotificationMessage>> GetNotificationListForAdmin()
        {
            Ensure.String.IsNotNullOrWhiteSpace(_user.AuthId);
            Ensure.Bool.IsTrue(_user.IsSuperAdmin);

            return await this._notificationMessageRepository.Find(nm => !nm.IsDeleted );
        }

        public async Task<ICollection<NotificationSummaryDO>> GetNotificationSummariesForUser()
        {
            Ensure.String.IsNotNullOrWhiteSpace(_user.AuthId);

            var summaries = await this.GetSummariesForUser(_user.AuthId);

            // Find those that are seen for the first time by this action and set them seen
            var toMarkAsSeen = summaries.Where(s => !s.HasBeenSeen);
            await this._notificationAudienceLogRepository.MarkManyAsSeen(toMarkAsSeen.Select(s => s.NotificationAudienceLogId));

            // From all summaries we know that those that must be opened are still valid for the check.
            await this.UpdateCheckInformationForUser(summaries);

            return summaries; 
        }

        private async Task UpdateCheckInformationForUser(ICollection<NotificationSummaryDO> summaries)
        {
            // If they get the message list and there's nothing left to open then there are no more to read
            var anyToConsiderNotRead = summaries.Any(s => s.CanOpen && !s.HasBeenOpened && !s.CanDismiss);
            if (!anyToConsiderNotRead)
            {
                await this._notificationChecker.SetAllMessagesReadForUser();
            }
            else
            {
                var maxSeverityOfStillToBeOpened = summaries.Where(s => s.CanOpen && !s.HasBeenOpened && !s.CanDismiss).Max(s => s.Severity);
                await this._notificationChecker.UpdateMaxSeverityForUser(maxSeverityOfStillToBeOpened);
            }
        }

        private async Task<ICollection<NotificationSummaryDO>> GetSummariesForUser(string authId)
        {
            var userNotifications = await this._notificationAudienceLogRepository.GetNotificationsForSummary();

            var messageSummaries = userNotifications.Select(x => new NotificationSummaryDO()
            {
                NotificationAudienceLogId = x.Id,
                Title = x.NotificationMessage.MessageHeader,
                Summary = x.NotificationMessage.MessageSummary,
                CanOpen = x.NotificationMessage.HasDetails,
                CanDismiss = x.AllowDismiss(),
                HasBeenOpened = x.DateOpenedOn != null,
                HasBeenSeen = x.DateSeenOn != null,
                HasBeenDeleted = x.IsDeleted,
                HasBeenAcknowledged = x.DateAcknowledgedOn != null,
                MustAcknowledge = x.NotificationMessage.RequiresAcknowledgement,
                Severity = x.NotificationMessage.Severity,
                NotificationType = x.NotificationMessage.NotificationType,
                DateCreated = x.NotificationMessage.DateCreated,
            });

            return messageSummaries.ToList(); // Unpack
        }
    }
}
