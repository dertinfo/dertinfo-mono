using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Repository
{
    public interface IMarkingSheetRepository : IRepository<MarkingSheetImage, int>
    {
        Task<ICollection<MarkingSheetImage>> GetMarkingSheetsExpandedByDanceId(int danceId);
    }

    public class MarkingSheetRepository : BaseRepository<MarkingSheetImage, int, DertInfoContext>, IMarkingSheetRepository
    {

        public MarkingSheetRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }

        public async Task<ICollection<MarkingSheetImage>> GetMarkingSheetsExpandedByDanceId(int danceId)
        {
            var query = _entitySet.Where(ms => ms.DanceId == danceId);

            query.Include(ms => ms.Image);

            return await query.ToListAsync();
        }
    }
}

