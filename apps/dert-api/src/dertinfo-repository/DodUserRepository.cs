using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IDodUserRepository : IRepository<DodUser, int>
    {
        Task<DodUser> GetByGuid(Guid guid);
        Task<ICollection<DodUser>> GetUsersWithResults();
        Task<DodUser> GetByIdWithResults(int userId);
    }

    public class DodUserRepository : BaseRepository<DodUser, int, DertInfoContext>, IDodUserRepository
    {
        public DodUserRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<DodUser> GetByGuid(Guid guid)
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodUser> query = _context.DodUser.Where(u => u.Guid == guid);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<DodUser> GetByIdWithResults(int userId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodUser> query = _context.DodUser.Include(u => u.DodResults.Where(r => !r.IsDeleted));

                return query.First(u => u.Id == userId);
            });

            return await task;
        }

        public async Task<ICollection<DodUser>> GetUsersWithResults()
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodUser> query = _context.DodUser
                .Where(u => !u.IsDeleted)
                .Include(u => u.DodResults.Where(r => !r.IsDeleted));
                 

                return query.ToList();
            });

            return await task;
        }
    }
}
