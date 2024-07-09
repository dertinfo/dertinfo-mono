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
    public interface IDodSubmissionRepository : IRepository<DodSubmission, int>
    {
        Task<ICollection<DodSubmission>> GetAllWithPrimaryImage();

        Task<ICollection<DodSubmission>> GetAllWithGroupAndResults();

        Task<DodSubmission> GetByIdWithPrimaryImage(int dodSubmissionId);
        Task<DodSubmission> GetSubmissionWithResultsByGroup(int groupId);
        Task<DodSubmission> GetSubmissionWithResultsById(int submissionId);
    }

    public class DodSubmissionRepository : BaseRepository<DodSubmission, int, DertInfoContext>, IDodSubmissionRepository
    {
        public DodSubmissionRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<DodSubmission>> GetAllWithGroupAndResults()
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodSubmission> query = _context.DodSubmission
                .Include(s => s.Group)
                .Include(s => s.DodResults);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<DodSubmission>> GetAllWithPrimaryImage()
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodSubmission> query = _context.DodSubmission
                .Include(s => s.Group).ThenInclude(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(gi => gi.Image);



                return query.ToList();
            });

            return await task;
        }

        public async Task<DodSubmission> GetByIdWithPrimaryImage(int dodSubmissionId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodSubmission> query = _context.DodSubmission.Where(s => s.Id == dodSubmissionId)
                 .Include(s => s.Group).ThenInclude(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(gi => gi.Image);

                return query.First();
            });

            return await task;
        }

        public async Task<DodSubmission> GetSubmissionWithResultsByGroup(int groupId)
        {
            var task = Task.Run(() =>
            {
                // We need to filter the include so we do this by projection
                IQueryable<DodSubmission> query = _context.DodSubmission
                                 .Where(s => s.GroupId == groupId && !s.IsDeleted)
                                 .Select(s => new DodSubmission
                                 {
                                     Id = s.Id,
                                     GroupId = s.GroupId,
                                     EmbedLink = s.EmbedLink,
                                     EmbedOrigin = s.EmbedOrigin,
                                     DodResults = s.DodResults.Where(r => r.IncludeInScores && !r.IsDeleted).ToList()
                                 });

                return query.First();
            });

            return await task;
        }

        public async Task<DodSubmission> GetSubmissionWithResultsById(int submissionId)
        {
            var task = Task.Run(() =>
            {
                // We need to filter the include so we do this by projection
                IQueryable<DodSubmission> query = _context.DodSubmission.Include(s => s.Group)
                                 .Where(s => s.Id == submissionId)
                                 .Select(s => new DodSubmission
                                 {
                                     Id = s.Id,
                                     GroupId = s.GroupId,
                                     EmbedLink = s.EmbedLink,
                                     EmbedOrigin = s.EmbedOrigin,
                                     DodResults = s.DodResults.Where(r => r.IncludeInScores && !r.IsDeleted).ToList(),
                                     Group = s.Group
                                 });

                return query.First();
            });

            return await task;
        }
    }
}
