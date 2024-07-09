using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IActivityTeamAttendanceRepository : IRepository<ActivityTeamAttendance, int>
    {
        Task<ActivityTeamAttendance> MarkDeleted(int id);
        Task<ActivityTeamAttendance> AddOrReinstate(ActivityTeamAttendance activityTeamAttendance);
        Task<IEnumerable<ActivityAttendanceDO>> GetTeamAttendancesByActivityId(int activityId);
    }

    public class ActivityTeamAttendanceRepository : BaseRepository<ActivityTeamAttendance, int, DertInfoContext>, IActivityTeamAttendanceRepository
    {
        public ActivityTeamAttendanceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<IEnumerable<ActivityAttendanceDO>> GetTeamAttendancesByActivityId(int activityId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.ActivityTeamAttendances
                .Where(ata => ata.ActivityId == activityId)
                .Select(ata => new
                {
                    ata.Id,
                    ata.Activity.Title,
                    ata.TeamAttendance.Team.TeamName,
                    ata.TeamAttendance.Team.Group.GroupName,
                    ata.Activity.Price,
                    ata.Activity.PriceTBC
                });

                var activityTeamAttendances = query.ToList();

                return activityTeamAttendances.Select(ata =>
                {
                    return new ActivityAttendanceDO
                    {
                        Id = ata.Id,
                        Name = ata.TeamName,
                        GroupName = ata.GroupName,
                        ActivityName = ata.Title,
                        SalesPrice = ata.Price,
                        SalesPriceTBC = ata.PriceTBC
                    };
                });
            });

            return await task;
        }

        public async Task<ActivityTeamAttendance> AddOrReinstate(ActivityTeamAttendance activityTeamAttendance)
        {
            var task = Task.Run(async () =>
            {
                ActivityTeamAttendance deletedActivityTeamAttendance = _context.ActivityTeamAttendances.IgnoreQueryFilters()
                    .Where(ma =>
                       ma.TeamAttendanceId == activityTeamAttendance.TeamAttendanceId &&
                       ma.ActivityId == activityTeamAttendance.ActivityId &&
                       ma.IsDeleted
                    ).FirstOrDefault();

                if (deletedActivityTeamAttendance != null)
                {
                    deletedActivityTeamAttendance.IsDeleted = false;
                // todo - reactivate any teamactivities

                activityTeamAttendance = deletedActivityTeamAttendance;

                    _context.SaveChanges();
                }
                else
                {
                    activityTeamAttendance = await Add(activityTeamAttendance);
                }

                return activityTeamAttendance;
            });

            return await task;
        }

        public async Task<ActivityTeamAttendance> MarkDeleted(int activityTeamAttendanceId)
        {

            var task = Task.Run(() =>
            {
                var activityTeamAttendance = _context.ActivityTeamAttendances.Find(activityTeamAttendanceId);

                activityTeamAttendance.IsDeleted = true;

                _context.SaveChanges();

                return activityTeamAttendance;
            });

            return await task;
        }
    }
}