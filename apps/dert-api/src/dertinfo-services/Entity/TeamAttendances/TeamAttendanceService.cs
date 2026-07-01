using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.TeamAttendances
{
    public interface ITeamAttendanceService
    {
        Task<ActivityTeamAttendance> RemoveTeamAttendanceActivity(int activityTeamAttendanceId);
        Task<IEnumerable<ActivityTeamAttendance>> RemoveTeamAttendanceActivities(int[] activityTeamAttendanceIds);
        Task<IEnumerable<ActivityTeamAttendance>> AddTeamActivities(int teamAttendanceId, int[] activityIds);
    }

    public class TeamAttendanceService : ITeamAttendanceService
    {
        IActivityRepository _activityRepository;
        IActivityTeamAttendanceRepository _activityTeamAttendanceRepository;
        ITeamAttendanceRepository _teamAttendanceRepository;

        public TeamAttendanceService(
            IActivityMemberAttendanceRepository activityMemberAttendanceRepository,
            IActivityRepository activityRepository,
            IActivityTeamAttendanceRepository activityTeamAttendanceRepository,
            IMemberAttendanceRepository memberAttendanceRepository,
            ITeamAttendanceRepository teamAttendanceRepository
            )
        {
            _activityRepository = activityRepository;
            _activityTeamAttendanceRepository = activityTeamAttendanceRepository;
            _teamAttendanceRepository = teamAttendanceRepository;

        }

        public async Task<IEnumerable<ActivityTeamAttendance>> AddTeamActivities(int teamAttendanceId, int[] activityIds)
        {
            List<ActivityTeamAttendance> activityTeamAttendances = new List<ActivityTeamAttendance>();
            foreach (var activityId in activityIds)
            {
                activityTeamAttendances.Add(await this.AddTeamActivity(teamAttendanceId, activityId));
            }

            return activityTeamAttendances;
        }

        public async Task<ActivityTeamAttendance> AddTeamActivity(int teamAttendanceId, int activityId)
        {
            // Protect from duplicate
            var activityTeamAttendance = await this._activityTeamAttendanceRepository.Find(ata => ata.ActivityId == activityId && ata.TeamAttendanceId == teamAttendanceId);
            if (activityTeamAttendance.FirstOrDefault() != null) { return activityTeamAttendance.First(); }

            var teamAttendance = await this._teamAttendanceRepository.GetById(teamAttendanceId);
            var activity = await this._activityRepository.GetById(activityId);

            // Only allow addition if the teams group matches the registrations group
            if (
                teamAttendance != null &&
                activity != null &&
                teamAttendance.Registration.EventId == activity.EventId &&
                activity.AudienceTypeId == (int)ActivityAudienceType.TEAM
                )
            {
                ActivityTeamAttendance newActivityTeamAttendance = new ActivityTeamAttendance();
                newActivityTeamAttendance.Activity = activity;
                newActivityTeamAttendance.TeamAttendance = teamAttendance;
                newActivityTeamAttendance.ActivityId = activity.Id;
                newActivityTeamAttendance.TeamAttendanceId = teamAttendance.Id;

                return await this._activityTeamAttendanceRepository.AddOrReinstate(newActivityTeamAttendance);
            }

            throw new ArgumentException("Cannot attach activity to team attendance where either team/activity does not exist or are not assosiated with the same type/event");
        }

        public async Task<IEnumerable<ActivityTeamAttendance>> RemoveTeamAttendanceActivities(int[] activityTeamAttendanceIds)
        {
            List<ActivityTeamAttendance> activityTeamAttendances = new List<ActivityTeamAttendance>();
            foreach (var activityTeamAttendanceId in activityTeamAttendanceIds)
            {
                activityTeamAttendances.Add(await this.RemoveTeamAttendanceActivity(activityTeamAttendanceId));
            }

            return activityTeamAttendances;
        }

        public async Task<ActivityTeamAttendance> RemoveTeamAttendanceActivity(int activityTeamAttendanceId)
        {
            return await _activityTeamAttendanceRepository.MarkDeleted(activityTeamAttendanceId);
        }
    }
}
