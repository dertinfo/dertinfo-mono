using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IEventSettingRepository : IRepository<EventSetting, int>
    {
        Task<ICollection<EventSetting>> GetByEventId(int eventId);
    }

    public class EventSettingRepository : BaseRepository<EventSetting, int, DertInfoContext>, IEventSettingRepository
    {
        public EventSettingRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<EventSetting>> GetByEventId(int eventId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.EventSettings
                .Where(es => es.EventId == eventId);
                
                return query.ToList();
            });

            return await task;
        }
    }
}
