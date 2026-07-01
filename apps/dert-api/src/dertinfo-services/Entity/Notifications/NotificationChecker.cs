using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Notifications
{

    public interface INotificationChecker
    {
        Task<NotificationThumbnailInfoDO> CheckForNotifications();
        Task<bool> SetAllMessagesReadForUser();
        Task<bool> UpdateMaxSeverityForUser(NotificationSeverity maxSeverityOfStillToBeOpened);
    }

    /// <summary>
    /// This Service is to evalue when a user has last seen thier messages. It checks to see if there are new messages since they last checked.
    /// If there are messages that have been added since they last checked then we should add those messages to the log for them to view.
    /// </summary>
    public class NotificationChecker : INotificationChecker
    {
        IAuthService _authService;
        INotificationAudienceLogRepository _notificationAudienceLogRepository;
        INotificationLastCheckRepository _notificationLastCheckRepository;
        INotificationMessageRepository _notificationMessageRepository;
        IDertInfoUser _user;

        public NotificationChecker(
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

        public async Task<NotificationThumbnailInfoDO> CheckForNotifications()
        {
            Ensure.String.IsNotNullOrWhiteSpace(_user.AuthId);

            var lastCheck = await this._notificationLastCheckRepository.SingleOrDefault(nlc => nlc.UserAuth0Identifier == _user.AuthId);
            var now = DateTime.Now;

            // Build an appropraite last check if there is no last check available.
            if (lastCheck == null)
            {
                lastCheck = await this.BuildLastCheckForNewUser(_user.AuthId);
            }

            var newMessageIdsAndSeverities = await this._notificationMessageRepository.GetIdsForNewMessagesInRange(lastCheck.LastCheckPerformedAt, now);
            // note - at some point in the furture we may add criteria to messages so that it applies to the subset of users. 
            //      - If the message would be appropriate for the user would be determined here before adding it to thier log.

            if (newMessageIdsAndSeverities.Count > 0)
            {
                var newMessageIds = newMessageIdsAndSeverities.Select(tple => tple.Item1).ToArray();
                var newMaxMessageSeverity = newMessageIdsAndSeverities.Max(tple => tple.Item2);
                var anyNewMessageIsBlocking = newMessageIdsAndSeverities.Any(tple => tple.Item3);

                if (anyNewMessageIsBlocking)
                {
                    lastCheck.HasBlocking = anyNewMessageIsBlocking;
                }

                lastCheck.HasUnreadMessages = true;
                lastCheck.MaximumMessageSeverity = (NotificationSeverity)Math.Max((int)lastCheck.MaximumMessageSeverity, (int)newMaxMessageSeverity);

                var logs = await this.AddNewMessagesForUserToLog(_user.AuthId, newMessageIds);

                if (anyNewMessageIsBlocking)
                {
                    lastCheck.HasBlocking = anyNewMessageIsBlocking;
                    lastCheck.BlockingNotificationId = newMessageIdsAndSeverities.Where(nm => nm.Item3).Last().Item1;
                    lastCheck.BlockingNotificationLogId = logs.Where(nal => nal.NotificationMessageId == lastCheck.BlockingNotificationId).Select(nal => nal.Id).Last();
                }
            }

            // Template the thumbnail Info
            var thumbnailInfo = new NotificationThumbnailInfoDO()
            {
                HasUnreadMessages = lastCheck.HasUnreadMessages,
                MaximumMessageSeverity = lastCheck.MaximumMessageSeverity,
                HasBlocking = lastCheck.HasBlocking,
                BlockingNotificationLogId = lastCheck.BlockingNotificationLogId,
                LatestCheck = now,
                PreviousCheck = lastCheck.LastCheckPerformedAt
            };

            // Update the last check datetime
            lastCheck.LastCheckPerformedAt = now;
            await this.AddOrUpdateLastCheck(lastCheck);

            return thumbnailInfo;
        }

        private async Task<NotificationLastCheck> BuildLastCheckForNewUser(string authId)
        {
            var userSettings = await this._authService.GetUserSettings(_user.AuthId);
            // note - this needs to be the date that the user was created but will require an Auth0 upgrade (at cost) to get this information.
            var checkStartAt = userSettings.GdprConsentGainedDate;
            return new NotificationLastCheck()
            {
                UserAuth0Identifier = _user.AuthId,
                LastCheckPerformedAt = checkStartAt ?? DateTime.Now,
                HasUnreadMessages = false
            };
        }

        private async Task<NotificationLastCheck> AddOrUpdateLastCheck(NotificationLastCheck lastCheck)
        {

            if (lastCheck.Id > 0)
            {
                await this._notificationLastCheckRepository.Update(lastCheck);
                return lastCheck;
            }
            else
            {
                return await this._notificationLastCheckRepository.Add(lastCheck);
            }
        }

        private async Task<IList<NotificationAudienceLog>> AddNewMessagesForUserToLog(string auth0UserIdentifier, int[] idsOfMessagesToAdd)
        {
            EnsureArg.IsNotNull(idsOfMessagesToAdd);

            if (idsOfMessagesToAdd.Length == 0) return new List<NotificationAudienceLog>();

            var logEntriesToAdd = new List<NotificationAudienceLog>();
            var now = DateTime.Now;

            foreach (var notificationMessageId in idsOfMessagesToAdd)
            {
                var newLogEntry = new NotificationAudienceLog()
                {
                    NotificationMessageId = notificationMessageId,
                    UserAuth0Identifier = auth0UserIdentifier,
                    DateDismissedOn = null,
                    DateClearedOn = null,
                    DateSeenOn = null,
                    DateNotifiedOn = now,
                    DateOpenedOn = null,
                    DateAcknowledgedOn = null
                };

                logEntriesToAdd.Add(newLogEntry);
            }

            return await this._notificationAudienceLogRepository.AddRange(logEntriesToAdd);
        }

        public async Task<bool> SetAllMessagesReadForUser()
        {
            return await this._notificationLastCheckRepository.SetAllMessagesReadForUser();
        }

        public async Task<bool> UpdateMaxSeverityForUser(NotificationSeverity maxSeverityOfStillToBeOpened)
        {
            return await this._notificationLastCheckRepository.UpdateMaxSeverityForUser(maxSeverityOfStillToBeOpened);
        }
    }

}
