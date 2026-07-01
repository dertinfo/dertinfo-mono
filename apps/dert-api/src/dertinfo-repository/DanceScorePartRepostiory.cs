using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IDanceScorePartRepository : IRepository<DanceScorePart, int>
    {
        Task<ICollection<DanceScorePartDO>> GetAllByDance(int danceId);
    }

    public class DanceScorePartRepository : BaseRepository<DanceScorePart, int, DertInfoContext>, IDanceScorePartRepository
    {
        public DanceScorePartRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {
        }

        public async Task<ICollection<DanceScorePartDO>> GetAllByDance(int danceId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.DanceScoreParts
                    .Where(dsp => dsp.DanceScore.DanceId == danceId)
                    .Include(dsp => dsp.DanceScore).ThenInclude(ds => ds.ScoreCategory).ThenInclude(sc => sc.ScoreSetScoreCategories)
                    .Include(dsp => dsp.JudgeSlot).ThenInclude(js => js.Judge).AsEnumerable()
                .Select(dsp => new DanceScorePartDO
                {
                    DanceScorePartId = dsp.Id,
                    DanceScoreId = dsp.DanceScore.Id,
                    JudgeId = dsp.JudgeSlot.Judge.Id,
                    JudgeName = dsp.JudgeSlot.Judge.Name,
                    JudgeSlotId = dsp.JudgeSlot.Id,
                    ScoreCategoryTag = dsp.DanceScore.ScoreCategory.Tag,
                    ScoreCategoryName = dsp.DanceScore.ScoreCategory.Name,
                    ScoreGiven = dsp.ScoreGiven,
                    SortOrder = dsp.DanceScore.ScoreCategory.SortOrder,
                    IsPartOfScoreSet = dsp.DanceScore.ScoreCategory.ScoreSetScoreCategories.Any(sssc => sssc.ScoreSetId == dsp.JudgeSlot.ScoreSetId)
                });

                var result = query.ToList();
                return result;
            });

            return await task;
        }
    }
}
