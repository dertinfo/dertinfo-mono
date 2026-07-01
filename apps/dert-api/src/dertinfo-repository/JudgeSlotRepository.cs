using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IJudgeSlotRepository : IRepository<JudgeSlot, int>
    {
        Task<JudgeSlot> GetByIdExpanded(int id);
        Task<RangeReportDO> GetJudgeSlotExpandedWithScorePartsById(int id);
    }

    public class JudgeSlotRepository : BaseRepository<JudgeSlot, int, DertInfoContext>, IJudgeSlotRepository
    {
        public JudgeSlotRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<JudgeSlot> GetByIdExpanded(int id)
        {
            var task = Task.Run(() =>
            {
                var query = _context.JudgeSlots
                .Where(js => js.Id == id)
                .Include(js => js.Judge)
                .Include(js => js.ScoreSet).ThenInclude(ss => ss.ScoreSetScoreCategories).ThenInclude(sssc => sssc.ScoreCategory);

                return query.FirstOrDefault();
            });

            return await task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Refer to: https://docs.microsoft.com/en-us/ef/core/querying/client-eval regading the as enumerable.
        /// It basically allows the query to get the data and then be processed on the client. Was causing an error without
        /// after the EF Core update.
        /// </remarks>
        public async Task<RangeReportDO> GetJudgeSlotExpandedWithScorePartsById(int id)
        {
            var task = Task.Run(() =>
            {
                var query = _context.JudgeSlots
                .Include(js => js.Judge)
                .Include(js => js.ScoreSet).ThenInclude(ss => ss.ScoreSetScoreCategories).ThenInclude(sssc => sssc.ScoreCategory)
                .Include(js => js.DanceScoreParts).ThenInclude(dsp => dsp.DanceScore).ThenInclude(ds => ds.ScoreCategory)
                .Where(js => js.Id == id)
                .AsEnumerable() 
                .Select(q => new RangeReportDO()
                {
                    JudgeId = q.Judge.Id,
                    JudgeName = q.Judge.Name,
                    ScoreRanges = q.DanceScoreParts
                                    .GroupBy(dsp => dsp.DanceScore.ScoreCategory)
                                    .Select(grp => new RangeReportSubRangeDO()
                                    {
                                        RangeName = grp.Key.Name,
                                        RangeTag = grp.Key.Tag,
                                        RangeValues = grp.Select(dsp => dsp.ScoreGiven)
                                    })


                });

                return query.FirstOrDefault();
            });

            return await task;
        }
    }
}
