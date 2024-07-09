using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface ICompetitionEntryRepository : IRepository<CompetitionEntry, int>
    {
        Task<CompetitionEntry> GetByIdExpandedByEntryAttributes(int competitionEntryId);
        Task<IEnumerable<CompetitionEntry>> GetByCompetitionId(int competitionId);
    }

    public class CompetitionEntryRepository : BaseRepository<CompetitionEntry, int, DertInfoContext>, ICompetitionEntryRepository
    {
        public CompetitionEntryRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        

        public async Task<CompetitionEntry> GetByIdExpandedByEntryAttributes(int competitionEntryId)
        {
            var task = Task.Run(() =>
            {
                return _context.CompetitionEntries.Where(ce => ce.Id == competitionEntryId)
                .Include(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries).FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<CompetitionEntry>> GetByCompetitionId(int competitionId)
        {
            var task = Task.Run(() =>
            {
                return _context.CompetitionEntries.Where(ce => ce.CompetitionId == competitionId)
                .Include(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries);
            });

            return await task;
        }
    }
}
