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
    public interface IDodResultComplaintRepository : IRepository<DodResultComplaint, int>
    {
        Task<ICollection<DodResultComplaint>> GetUnresolvedWithResult();
        Task<ICollection<DodResultComplaint>> GetResolvedWithResult();
    }

    public class DodResultComplaintRepository : BaseRepository<DodResultComplaint, int, DertInfoContext>, IDodResultComplaintRepository
    {
        public DodResultComplaintRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<DodResultComplaint>> GetResolvedWithResult()
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodResultComplaint> query = _context.DodResultComplaint.Where(rc => rc.IsResolved)
                .Include(rc => rc.DodResult);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<DodResultComplaint>> GetUnresolvedWithResult()
        {
            var task = Task.Run(() =>
            {
                IQueryable<DodResultComplaint> query = _context.DodResultComplaint.Where(rc => !rc.IsResolved)
                .Include(rc => rc.DodResult);

                return query.ToList();
            });

            return await task;
        }
    }
}
