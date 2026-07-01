using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IDodResultRepository : IRepository<DodResult, int>
    {
        Task<ICollection<DodResult>> GetAllOfficial();
        Task<ICollection<DodResult>> GetAllUnOfficial();
        Task<ICollection<int>> GetDanceIdsJudgedByUser(int userId);
        Task<DodResult> GetByIdWithSubmission(int resultId);
    }

    public class DodResultRepository : BaseRepository<DodResult, int, DertInfoContext>, IDodResultRepository
    {
        public DodResultRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<DodResult>> GetAllOfficial()
        {
            var task = Task.Run(() =>
            {
                // todo - this is really inefficient to load all the results with the submission in order to gain whether it has results. 
                //      - we need to refator this as there could be 1000's of result sets. 

                IQueryable<DodResult> query = _context.DodResult
                .Include(s => s.Submission).ThenInclude(s => s.Group);

                return query.Where(r => r.IsOfficial == true).ToList();
            });

            return await task;
        }

        public async Task<ICollection<DodResult>> GetAllUnOfficial()
        {
            var task = Task.Run(() =>
            {
                // todo - this is really inefficient to load all the results with the submission in order to gain whether it has results. 
                //      - we need to refator this as there could be 1000's of result sets. 

                IQueryable<DodResult> query = _context.DodResult
                .Include(s => s.Submission).ThenInclude(s => s.Group);

                return query.Where(r => r.IsOfficial == false).ToList();
            });

            return await task;
        }

        public async Task<DodResult> GetByIdWithSubmission(int resultId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodResult> query = _context.DodResult
                .Include(s => s.Submission);

                return query.First(r => r.Id == resultId);
            });

            return await task;
        }

        public async Task<ICollection<int>> GetDanceIdsJudgedByUser(int userId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<int> query = _context.DodResult.Where(r => r.DodUserId == userId).Select(r => r.SubmissionId);

                return query.Distinct().ToList();
            });

            return await task;
        }
    }
}
