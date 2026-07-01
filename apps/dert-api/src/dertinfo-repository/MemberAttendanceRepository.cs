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
    public interface IMemberAttendanceRepository : IRepository<MemberAttendance, int>
    {
        Task<MemberAttendance> MarkDeleted(int id);
        Task<MemberAttendance> AddOrReinstate(MemberAttendance activityMemberAttendance);
        Task<decimal> SumAttendanceSalesForRegistration(int registrationId);
    }

    public class MemberAttendanceRepository : BaseRepository<MemberAttendance, int, DertInfoContext>, IMemberAttendanceRepository
    {
        public MemberAttendanceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<MemberAttendance> AddOrReinstate(MemberAttendance memberAttendance)
        {
            var task = Task.Run(async () =>
            {
                MemberAttendance deletedMemberAttendance = _context.MemberAttendances.IgnoreQueryFilters()
                    .Where(ma =>
                       ma.GroupMemberId == memberAttendance.GroupMemberId &&
                       ma.RegistrationId == memberAttendance.RegistrationId &&
                       ma.IsDeleted
                    ).FirstOrDefault();

                if (deletedMemberAttendance != null)
                {
                    deletedMemberAttendance.IsDeleted = false;
                    // todo - reactivate any memberactivities

                    memberAttendance = deletedMemberAttendance;

                    _context.SaveChanges();
                }
                else
                {
                    memberAttendance = await Add(memberAttendance);
                }

                IQueryable<MemberAttendance> query = _context.MemberAttendances
                .Where(ma => ma.Id == memberAttendance.Id)
                .Include(ma => ma.MemberActivities).ThenInclude(memberActivity => memberActivity.Activity);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<MemberAttendance> MarkDeleted(int memberAttendanceId)
        {

            var task = Task.Run(() =>
            {
                var memberAttendance = _context.MemberAttendances.Find(memberAttendanceId);

                memberAttendance.IsDeleted = true;
                // todo - deactivate any memberactivities

                _context.SaveChanges();

                return memberAttendance;
            });

            return await task;
        }

        public async Task<decimal> SumAttendanceSalesForRegistration(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var memberAttendances = _context.MemberAttendances
                .Where(mAtt => mAtt.RegistrationId == registrationId)
                .Include(ma => ma.MemberActivities).ThenInclude(mAct => mAct.Activity);

                var attendances = memberAttendances.IgnoreQueryFilters();
                attendances = attendances.Where(ma => !ma.IsDeleted);

                decimal totalMemberPrice = 0;
                foreach (var memberAttendance in attendances.ToList())
                {
                    if (memberAttendance.MemberActivities != null)
                    {
                        totalMemberPrice += memberAttendance.MemberActivities.Where(ma => !ma.IsDeleted).Sum(mAct => mAct.Activity.Price);
                    }
                }

                return totalMemberPrice;
            });

            return await task;
        }
    }
}