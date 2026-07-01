using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Repository
{
    public interface IJudgeRepository : IRepository<Judge, int>
    {
        Task<IEnumerable<Judge>> GetJudgesByCompetition(int competitionId);
        Task<Judge> GetByIdExpandedByAttachments(int judgeId);
        Task<JudgeSlot> ClearJudgeAllocationFromSlot(int judgeSlotId);
    }

    public class JudgeRepository : BaseRepository<Judge, int, DertInfoContext>, IJudgeRepository
    {
        public JudgeRepository(DertInfoContext context, IDertInfoUser user) : base(context, user) { }

        public async Task<Judge> GetByIdExpandedByAttachments(int judgeId)
        {
            var task = Task.Run(() =>
            {
                return _context.Judges.Where(j => j.Id == judgeId)
                .Include(j => j.CompetitionJudges)
                .Include(j => j.JudgeSlots)
                .Include(j => j.EventJudges).FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<Judge>> GetJudgesByCompetition(int competitionId)
        {
            var task = Task.Run(() =>
            {
                return _context.Judges.Where(j => j.CompetitionJudges.Any(cj => cj.CompetitionId == competitionId)).ToList();
            });

            return await task;
        }

        public async Task<JudgeSlot> ClearJudgeAllocationFromSlot(int judgeSlotId)
        {
            var task = Task.Run(() =>
            {
                var judgeSlot = _context.JudgeSlots.Find(judgeSlotId);

                judgeSlot.Judge = null;
                judgeSlot.JudgeId = null;

                _context.SaveChanges();

                return judgeSlot;

            });

            return await task;
        }
    }
}
