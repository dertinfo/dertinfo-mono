using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface INotificationLastCheckRepository : IRepository<NotificationLastCheck, string>
    {
        Task<bool> SetAllMessagesReadForUser();
        Task<bool> UpdateMaxSeverityForUser(NotificationSeverity maxSeverity);
    }

    public class NotificationLastCheckRepository : BaseRepository<NotificationLastCheck, string, DertInfoContext>, INotificationLastCheckRepository
    {
        public NotificationLastCheckRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }

        public async Task<bool> SetAllMessagesReadForUser()
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationLastCheck> query = _context.NotificationLastChecks
                .Where(nlc => nlc.UserAuth0Identifier == _user.AuthId);

                var checkEntry = query.First();
                checkEntry.HasUnreadMessages = false;
                checkEntry.MaximumMessageSeverity = NotificationSeverity.None;

                _context.SaveChanges();

                return true;
            });

            return await task;
        }

        public async Task<bool> UpdateMaxSeverityForUser(NotificationSeverity maxSeverity)
        {
            var task = Task.Run(() =>
            {
                IQueryable<NotificationLastCheck> query = _context.NotificationLastChecks
                .Where(nlc => nlc.UserAuth0Identifier == _user.AuthId);

                var checkEntry = query.First();
                checkEntry.MaximumMessageSeverity = maxSeverity;

                _context.SaveChanges();

                return true;
            });

            return await task;
        }
    }
}
