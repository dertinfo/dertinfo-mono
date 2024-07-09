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
    public interface ICompetitionScoreSetRepository : IRepository<ScoreSet, int>
    {
        Task AttachScoreCategory(ScoreSet scoreSet, ScoreCategory scoreCategory);
        Task<IEnumerable<ScoreSet>> GetScoreSets(int competitionId);
    }

    public class CompetitionScoreSetRepository : BaseRepository<ScoreSet, int, DertInfoContext>, ICompetitionScoreSetRepository
    {
        public CompetitionScoreSetRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task AttachScoreCategory(ScoreSet scoreSet, ScoreCategory scoreCategory)
        {
            ScoreSetScoreCategory ss_sc = new ScoreSetScoreCategory();
            ss_sc.ScoreSet = scoreSet;
            ss_sc.ScoreCategory = scoreCategory;
            await this._context.ScoreSetScoreCategories.AddAsync(ss_sc);
            await this._context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ScoreSet>> GetScoreSets(int competitionId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<ScoreSet> query = _context.ScoreSets
                .Where(ss => ss.CompetitionId == competitionId)
                .Include(ss => ss.ScoreSetScoreCategories).ThenInclude(sssc => sssc.ScoreCategory);

                return query.ToList();
            });

            return await task;
        }
    }
}
