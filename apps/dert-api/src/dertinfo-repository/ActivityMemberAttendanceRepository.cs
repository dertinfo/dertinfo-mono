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
    public interface IActivityMemberAttendanceRepository : IRepository<ActivityMemberAttendance, int>
    {
        Task<ActivityMemberAttendance> MarkDeleted(int id);
        Task<ActivityMemberAttendance> AddOrReinstate(ActivityMemberAttendance memberAttendance);

        Task<IEnumerable<ActivityAttendanceDO>> GetMemberAttendancesByActivityId(int activityId);
    }

    public class ActivityMemberAttendanceRepository : BaseRepository<ActivityMemberAttendance, int, DertInfoContext>, IActivityMemberAttendanceRepository
    {
        public ActivityMemberAttendanceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<IEnumerable<ActivityAttendanceDO>> GetMemberAttendancesByActivityId(int activityId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.ActivityMemberAttendances
                .Where(ata => ata.ActivityId == activityId)
                .Select(ata => new
                {
                    ata.Id,
                    ata.Activity.Title,
                    ata.MemberAttendance.GroupMember.Name,
                    ata.MemberAttendance.GroupMember.Group.GroupName,
                    ata.Activity.Price,
                    ata.Activity.PriceTBC
                });

                var activityTeamAttendances = query.ToList();

                return activityTeamAttendances.Select(ata =>
                {
                    return new ActivityAttendanceDO
                    {
                        Id = ata.Id,
                        Name = ata.Name,
                        GroupName = ata.GroupName,
                        ActivityName = ata.Title,
                        SalesPrice = ata.Price,
                        SalesPriceTBC = ata.PriceTBC
                    };
                });
            });

            return await task;
        }

        public async Task<ActivityMemberAttendance> AddOrReinstate(ActivityMemberAttendance activityMemberAttendance)
        {
            var task = Task.Run(async () =>
            {
                ActivityMemberAttendance deletedActivityMemberAttendance = _context.ActivityMemberAttendances.IgnoreQueryFilters()
                    .Where(ma =>
                       ma.MemberAttendanceId == activityMemberAttendance.MemberAttendanceId &&
                       ma.ActivityId == activityMemberAttendance.ActivityId &&
                       ma.IsDeleted
                    ).FirstOrDefault();

                if (deletedActivityMemberAttendance != null)
                {
                    deletedActivityMemberAttendance.IsDeleted = false;
                    // todo - reactivate any memberactivities

                    activityMemberAttendance = deletedActivityMemberAttendance;

                    _context.SaveChanges();
                }
                else
                {
                    activityMemberAttendance = await Add(activityMemberAttendance);
                }

                return activityMemberAttendance;
            });

            return await task;
        }

        public async Task<ActivityMemberAttendance> MarkDeleted(int activityMemberAttendanceId)
        {

            var task = Task.Run(() =>
            {
                var activityMemberAttendance = _context.ActivityMemberAttendances.Find(activityMemberAttendanceId);

                activityMemberAttendance.IsDeleted = true;
                // todo - deactivate any memberactivities

                _context.SaveChanges();

                return activityMemberAttendance;
            });

            return await task;
        }
    }
}