using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface INotificationAudienceLogRepository : IRepository<NotificationAudienceLog, int>
    {
        Task<IQueryable<NotificationAudienceLog>> GetNotificationsForSummary();
        Task<bool> MarkManyAsSeen(IEnumerable<int> idsToMarkSeen);
        Task<bool> Dismiss(int id);
        Task<bool> Acknowledge(int id);
        Task<bool> Clear(int id);
        Task<NotificationAudienceLog> GetDetailForUser(int id);
        Task<bool> MarkAsOpened(int notificationAudienceLogId);
        Task<bool> ApplyDeletedToLogsByNotificationMessageId(int notificationMessageId);
    }

    public class NotificationAudienceLogRepository : BaseRepository<NotificationAudienceLog, int, DertInfoContext>, INotificationAudienceLogRepository
    {
        public NotificationAudienceLogRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }

        /// <summary>
        /// Dismisses the notification. 
        /// Notifications can only be dismissed if the current user it the owner of the message.
        /// To dismiss the message does not have to be read. Messages that are cleared must have a read date.
        /// </summary>
        /// <param name="id">The id of the message.</param>
        /// <returns></returns>
        /// <remarks>The notification must be dismissable. If not dismissable then we should throw an error. A notification can be dismissed if it has been acknowledged</remarks>
        public async Task<bool> Dismiss(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.UserAuth0Identifier == _user.AuthId && nal.Id == id);

                var notificationAudienceLog = query.First();
                notificationAudienceLog.DateDismissedOn = DateTime.Now;

                _context.SaveChanges();

                return true;
            });

            return await task;

        }

        public async Task<bool> Acknowledge(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.UserAuth0Identifier == _user.AuthId && nal.Id == id);

                var notificationAudienceLog = query.First();
                notificationAudienceLog.DateAcknowledgedOn = DateTime.Now;

                _context.SaveChanges();

                return true;
            });

            return await task;

        }


        /// <summary>
        /// For a message to be cleared it must first have been read. If it has not been read then they are not permitted to clear it if it has details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Clear(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.UserAuth0Identifier == _user.AuthId && nal.Id == id);

                var notificationAudienceLog = query.First();
                notificationAudienceLog.DateClearedOn = DateTime.Now;

                _context.SaveChanges();

                return true;
            });

            return await task;
        }

        /// <summary>
        /// Get the detail of a message. User can only get detail if thier own message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NotificationAudienceLog> GetDetailForUser(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.UserAuth0Identifier == _user.AuthId && nal.Id == id)
                .Include(nal => nal.NotificationMessage);

                return query.First();
            });

            return await task;
        }

        /// <summary>
        /// This function eager loads the messages associated with the messages for the user. 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IQueryable<NotificationAudienceLog>> GetNotificationsForSummary()
        {
            var task = Task.Run(() =>
            {
                // Get the message summaries where: 
                // - the user has not dismissed it or cleared it
                // - and it's not deleted ot they have seen it and it is deleted. 
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.UserAuth0Identifier == _user.AuthId && nal.DateDismissedOn == null && nal.DateClearedOn == null && (!nal.IsDeleted || nal.DateSeenOn != null))
                .Include(nal => nal.NotificationMessage);

                return query;
            });

            return await task;
        }

        public async Task<bool> MarkManyAsSeen(IEnumerable<int> idsToMarkSeen)
        {
            var task = Task.Run(() =>
            {
                /* This performs a basic update access several rows. it runs it for each */
                /*
                 * UPDATE [NotificationAudienceLogs] SET [DateModified] = @p0, [DateSeenOn] = @p1 WHERE [Id] = @p2;
                 * @p0,@p1 = DateTime.Now
                 * @p2 - the id of the row.
                 * It runs this SQL for each instance.
                 */
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                    .Where(nal => idsToMarkSeen.Contains(nal.Id) && nal.UserAuth0Identifier == _user.AuthId);
                // note - there should be a way to do this with a single call. 
                //      - Not bothered enough to chase this down at the moment.
                //      - the performance difference will be negligable unless there is a lot of messages.
                //      - or a lot of users.
                var now = DateTime.Now;

                foreach (var item in query)
                {
                    item.DateSeenOn = now;
                    item.DateModified = now;
                    item.ModifiedBy = _user.AuthId;
                }

                _context.SaveChanges();

                return true;
            });

            return await task;
        }

        public async Task<bool> MarkAsOpened(int notificationAudienceLogId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                    .Where(nal => nal.Id == notificationAudienceLogId && nal.UserAuth0Identifier == _user.AuthId);
                
                var now = DateTime.Now;

                foreach (var item in query)
                {
                    item.DateSeenOn = now;
                    item.DateOpenedOn = now;
                    item.DateModified = now;
                    item.ModifiedBy = _user.AuthId;
                }

                _context.SaveChanges();

                return true;
            });

            return await task;
        }

        public async Task<bool> ApplyDeletedToLogsByNotificationMessageId(int notificationMessageId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationAudienceLog> query = _context.NotificationAudienceLogs
                .Where(nal => nal.NotificationMessageId == notificationMessageId && !nal.IsDeleted);

                var now = DateTime.Now;

                foreach (var item in query)
                {
                    item.DateModified = now;
                    item.ModifiedBy = _user.AuthId;
                    item.IsDeleted = true;
                }

                _context.SaveChanges();

                return true;
            });

            return await task;
        }
    }
}
