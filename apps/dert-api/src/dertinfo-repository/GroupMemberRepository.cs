using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Repository
{
    public interface IGroupMemberRepository : IRepository<GroupMember, int>
    {
        Task<ICollection<GroupMember>> GetByGroupWithAttendance(int groupId);
        Task<GroupMember> MarkDeleted(int id);
        Task<GroupMember> GetGroupMemberDetail(int groupMemberId);
    }

    public class GroupMemberRepository : BaseRepository<GroupMember, int, DertInfoContext>, IGroupMemberRepository
    {
        public GroupMemberRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<GroupMember>> GetByGroupWithAttendance(int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<GroupMember> query = _context.GroupMembers
                .Where(gm => gm.GroupId == groupId && !gm.IsDeleted)
                .Include(gm => gm.MemberAttendances).ThenInclude(ma => ma.MemberActivities).ThenInclude(mac => mac.Activity);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Team>> GetAllWithImagesAndRegistrations()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<GroupMember> MarkDeleted(int groupMemberId) {

            var task = Task.Run(() =>
            {
                var groupMember = _context.GroupMembers.Find(groupMemberId);

                groupMember.IsDeleted = true;

                _context.SaveChanges();

                return groupMember;
            });

            return await task;
        }

        public async Task<GroupMember> GetGroupMemberDetail(int id) {

            var task = Task.Run(() =>
            {
                IQueryable<GroupMember> query = _context.GroupMembers
                .Where(gm => gm.Id == id)
                .Include(gm => gm.MemberAttendances).ThenInclude(ma => ma.MemberActivities).ThenInclude(mac => mac.Activity)
                .Include(gm => gm.MemberAttendances).ThenInclude(ma => ma.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(ei => ei.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }
    }
}
