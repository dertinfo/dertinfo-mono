using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.MemberAttendance
{

    public interface IMemberAttendanceService
    {
        Task<ActivityMemberAttendance> RemoveMemberAttendanceActivity(int activityMemberAttendanceId);
        Task<IEnumerable<ActivityMemberAttendance>> RemoveMemberAttendanceActivities(int[] activityMemberAttendanceIds);
        Task<IEnumerable<ActivityMemberAttendance>> AddMemberActivities(int memberAttendanceId, int[] activityIds);
    }

    public class MemberAttendanceService : IMemberAttendanceService
    {

        IActivityMemberAttendanceRepository _activityMemberAttendanceRepository;
        IActivityRepository _activityRepository;
        IMemberAttendanceRepository _memberAttendanceRepository;

        public MemberAttendanceService(
            IActivityMemberAttendanceRepository activityMemberAttendanceRepository,
            IActivityRepository activityRepository,
            IActivityTeamAttendanceRepository activityTeamAttendanceRepository,
            IMemberAttendanceRepository memberAttendanceRepository,
            ITeamAttendanceRepository teamAttendanceRepository
            )
        {
            _activityMemberAttendanceRepository = activityMemberAttendanceRepository;
            _activityRepository = activityRepository;    
            _memberAttendanceRepository = memberAttendanceRepository;
        }

        public async Task<IEnumerable<ActivityMemberAttendance>> AddMemberActivities(int memberAttendanceId, int[] activityIds)
        {

            List<ActivityMemberAttendance> activityMemberAttendances = new List<ActivityMemberAttendance>();
            foreach (var activityId in activityIds)
            {
                activityMemberAttendances.Add(await this.AddMemberActivity(memberAttendanceId, activityId));
            }

            return activityMemberAttendances;

        }

        public async Task<ActivityMemberAttendance> AddMemberActivity(int memberAttendanceId, int activityId)
        {
            // Protect from duplicate
            var activityMemberAttendance = await this._activityMemberAttendanceRepository.Find(ama => ama.ActivityId == activityId && ama.MemberAttendanceId == memberAttendanceId);
            if (activityMemberAttendance.FirstOrDefault() != null) { return activityMemberAttendance.First(); }

            var memberAttendance = await this._memberAttendanceRepository.GetById(memberAttendanceId);
            var activity = await this._activityRepository.GetById(activityId);

            if (
                memberAttendance != null &&
                activity != null &&
                memberAttendance.Registration.EventId == activity.EventId &&
                activity.AudienceTypeId == (int)ActivityAudienceType.INDIVIDUAL
                )
            {
                ActivityMemberAttendance newActivityMemberAttendance = new ActivityMemberAttendance();
                newActivityMemberAttendance.Activity = activity;
                newActivityMemberAttendance.MemberAttendance = memberAttendance;
                newActivityMemberAttendance.ActivityId = activity.Id;
                newActivityMemberAttendance.MemberAttendanceId = memberAttendance.Id;

                return await this._activityMemberAttendanceRepository.AddOrReinstate(newActivityMemberAttendance);
            }

            throw new ArgumentException("Cannot attach activity to member attendance where either member/activity does not exist or are not assosiated with the same type/event");
        }

        public async Task<IEnumerable<ActivityMemberAttendance>> RemoveMemberAttendanceActivities(int[] activityMemberAttendanceIds)
        {
            List<ActivityMemberAttendance> activityMemberAttendances = new List<ActivityMemberAttendance>();
            foreach (var activityMemberAttendanceId in activityMemberAttendanceIds)
            {
                activityMemberAttendances.Add(await this.RemoveMemberAttendanceActivity(activityMemberAttendanceId));
            }

            return activityMemberAttendances;
        }

        public async Task<ActivityMemberAttendance> RemoveMemberAttendanceActivity(int activityMemberAttendanceId)
        {
            return await _activityMemberAttendanceRepository.MarkDeleted(activityMemberAttendanceId);
        }
    }

}
