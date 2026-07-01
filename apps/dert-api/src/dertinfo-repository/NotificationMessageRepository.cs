using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface INotificationMessageRepository : IRepository<NotificationMessage, int>
    {
        Task<IList<Tuple<int, NotificationSeverity, bool>>> GetIdsForNewMessagesInRange(DateTime lastCheckPerformedAt, DateTime now);
    }

    public class NotificationMessageRepository : BaseRepository<NotificationMessage, int, DertInfoContext>, INotificationMessageRepository
    {
        public NotificationMessageRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }

        public async Task<IList<Tuple<int, NotificationSeverity, bool>>> GetIdsForNewMessagesInRange(DateTime lastCheckPerformedAt, DateTime now)
        {
            var task = Task.Run(() =>
            {
                return _context.NotificationMessages
                    .Where(nm => nm.DateCreated >= lastCheckPerformedAt && nm.DateCreated <= now)
                    .Select(nm => new Tuple<int,NotificationSeverity, bool>(nm.Id, nm.Severity, nm.BlocksUser))
                    .ToList(); // UnPack
            });

            return await task;
        }
    }
}
