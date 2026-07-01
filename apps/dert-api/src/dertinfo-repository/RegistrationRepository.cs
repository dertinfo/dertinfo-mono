using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Repository
{
    public interface IRegistrationRepository : IRepository<Registration, int>
    {
        Task<ICollection<Registration>> GetByGroupWithEventImagesAndCounts(int groupId);
        Task<ICollection<Registration>> GetByEventWithGroupImagesAndCounts(int eventId, bool removeDeleted = true);
        Task<bool> IsAlreadyRegistered(int groupId, int eventId);
        Task<ICollection<MemberAttendance>> GetMemberAttendances(int registrationId);
        Task<ICollection<TeamAttendance>> GetTeamAttendances(int registrationId);
        Task<Registration> GetOverviewByGroupPerspective(int registrationId);
        Task<Registration> GetOverviewByEventPerspective(int registrationId);
        Task<Registration> GetForEmail(int registrationId);
        Task<IEnumerable<Registration>> GetForSignInSheets(int eventId);
    }

    public class RegistrationRepository : BaseRepository<Registration, int, DertInfoContext>, IRegistrationRepository
    {
        public RegistrationRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {
        }

        public async Task<ICollection<Registration>> GetByGroupWithEventImagesAndCounts(int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Registration> query = _context.Registrations
                .Where(r => r.GroupId == groupId)
                .Include(r => r.TeamAttendances)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember)
                .Include(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image);

                var results = query.IgnoreQueryFilters().ToList();

                /* Stripping Unwanted - due to ignore query filters*/
                foreach (var result in results)
                {
                    result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();
                    result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                }

                return results;
            });

            return await task;
        }

        public async Task<ICollection<Registration>> GetByEventWithGroupImagesAndCounts(int eventId, bool removeDeleted = true)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Registration> query = _context.Registrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.TeamAttendances)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember)
                .Include(r => r.Group).ThenInclude(e => e.GroupImages).ThenInclude(i => i.Image);

                var results = query.IgnoreQueryFilters().ToList();

                /* Stripping Unwanted - due to ignore query filters*/
                if (removeDeleted)
                {
                    foreach (var result in results)
                    {
                        result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                        result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();
                    }
                }

                return results;

            });

            return await task;
        }

        public async Task<bool> IsAlreadyRegistered(int groupId, int eventId)
        {
            var task = Task.Run(() =>
            {
                var alreadyRegistered = _context.Registrations.Any(r => r.GroupId == groupId && r.EventId == eventId);
                return alreadyRegistered;
            });

            return await task;
        }

        public async Task<ICollection<MemberAttendance>> GetMemberAttendances(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.MemberAttendances
                .Where(mAtt => mAtt.RegistrationId == registrationId)
                .Include(mAtt => mAtt.GroupMember)
                .Include(mAtt => mAtt.MemberActivities).ThenInclude(mActs => mActs.Activity);

                var result = query.IgnoreQueryFilters().ToList();

                /* Stripping Unwanted - due to ignore query filters*/
                foreach (var ma in result)
                {
                    ma.MemberActivities = ma.MemberActivities.Where(mAct => !mAct.IsDeleted).ToList();
                }

                return result.Where(ma => !ma.IsDeleted).ToList();

            });

            return await task;
        }

        public async Task<ICollection<TeamAttendance>> GetTeamAttendances(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.TeamAttendances
                .Where(mAtt => mAtt.RegistrationId == registrationId)
                .Include(mAtt => mAtt.Team)
                .Include(mAtt => mAtt.TeamActivities).ThenInclude(mActs => mActs.Activity);

                var result = query.IgnoreQueryFilters().ToList();

                /* Stripping Unwanted - due to ignore query filters*/
                foreach (var ta in result)
                {
                    ta.TeamActivities = ta.TeamActivities.Where(tAct => !tAct.IsDeleted).ToList();
                }

                return result.Where(ta => !ta.IsDeleted).ToList();
            });



            return await task;
        }

        public async Task<Registration> GetOverviewByEventPerspective(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Registrations
                .Where(r => r.Id == registrationId)

                .Include(r => r.TeamAttendances).ThenInclude(ta => ta.Team)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember)
                .Include(r => r.Event).ThenInclude(g => g.EventImages).ThenInclude(ei => ei.Image)
                .Include(r => r.Group).ThenInclude(g => g.GroupImages).ThenInclude(gi => gi.Image);

                var result = query.IgnoreQueryFilters().FirstOrDefault();

                /* Stripping Unwanted - due to ignore query filters*/
                result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();


                return result;
            });

            return await task;
        }

        /// <summary>
        /// Gets the overview for the registration.
        /// 
        /// note - by joining a deleted obejct to it's join i.e. the attendance then the attendance is not returned.
        ///      - in this case this is desirable as when the Member is marked deleted this also removes the attendance from the return.
        ///      - we need to be careful about this.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        public async Task<Registration> GetOverviewByGroupPerspective(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Registrations
                .Where(r => r.Id == registrationId)
                .Include(r => r.Group)
                .Include(r => r.TeamAttendances).ThenInclude(ta => ta.Team) // This then include action causes the team attendance not to be returned if the team is null.
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember) // This then include action causes the member attendance not to be returned if the member is null.
                .Include(r => r.Group).ThenInclude(g => g.GroupImages).ThenInclude(gi => gi.Image)
                .Include(r => r.Event).ThenInclude(g => g.EventImages).ThenInclude(gi => gi.Image); // This then include action causes the whole thing  not to be returned if the event is null.

                var result = query.IgnoreQueryFilters().FirstOrDefault();

                /* Stripping Unwanted - due to ignore query filters*/
                result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();

                return result;
            });

            return await task;
        }

        public async Task<Registration> GetForEmail(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Registrations.AsNoTracking()
                .Where(r => r.Id == registrationId)
                .Include(r => r.TeamAttendances).ThenInclude(ta => ta.Team) // This then include action causes the team attendance not to be returned if the team is null.
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember) // This then include action causes the member attendance not to be returned if the member is null.
                .Include(r => r.TeamAttendances).ThenInclude(ma => ma.TeamActivities).ThenInclude(ta => ta.Activity)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.MemberActivities).ThenInclude(ta => ta.Activity)
                .Include(r => r.Group).ThenInclude(g => g.GroupImages).ThenInclude(gi => gi.Image)
                .Include(r => r.Event).ThenInclude(g => g.EventImages).ThenInclude(gi => gi.Image); // This then include action causes the whole thing  not to be returned if the event is null.

                var result = query.IgnoreQueryFilters().FirstOrDefault();

                /* Stripping Unwanted - due to ignore query filters*/
                foreach (var ma in result.MemberAttendances)
                {
                    ma.MemberActivities = ma.MemberActivities.Where(tAct => !tAct.IsDeleted).ToList();
                }

                foreach (var ta in result.TeamAttendances)
                {
                    ta.TeamActivities = ta.TeamActivities.Where(tAct => !tAct.IsDeleted).ToList();
                }


                result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();

                return result;
            });

            return await task;
        }

        public async Task<IEnumerable<Registration>> GetForSignInSheets(int eventId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Registration> query = _context.Registrations
                .Where(r => r.EventId == eventId && ( r.FlowState == RegistrationFlowState.Confirmed || r.FlowState == RegistrationFlowState.Closed))
                .Include(r => r.TeamAttendances)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.GroupMember)
                .Include(r => r.MemberAttendances).ThenInclude(ma => ma.MemberActivities).ThenInclude(ama => ama.Activity)
                .Include(r => r.Group)
                .Include(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image);

                var results = query.IgnoreQueryFilters().ToList();

                /* Stripping Unwanted - due to ignore query filters*/
                foreach (var result in results)
                {
                    result.MemberAttendances = result.MemberAttendances.Where(ma => !ma.IsDeleted).ToList();

                    foreach (var memberAttendance in result.MemberAttendances)
                    {
                        memberAttendance.MemberActivities = memberAttendance.MemberActivities.Where(a => !a.IsDeleted).ToList();
                    }

                    result.TeamAttendances = result.TeamAttendances.Where(ta => !ta.IsDeleted).ToList();
                }

                return results;
            });

            return await task;
        }
    }
}
