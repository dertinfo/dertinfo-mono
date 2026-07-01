using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Activities
{
    public interface IActivityService
    {
        Task<Activity> UpdateActivity(Activity updatedActivity);
        Task<Activity> GetDetail(int activityId);
        Task<IEnumerable<Activity>> GetDefaultsForEvent(int eventId, ActivityAudienceType audienceType);
        Task<IEnumerable<ActivityAttendanceDO>> GetActivityAttendances(int activityId);
    }
    public class ActivityService : IActivityService
    {
        IActivityMemberAttendanceRepository _activityMemberAttendanceRepository;
        IActivityRepository _activityRepository;
        IActivityTeamAttendanceRepository _activityTeamAttendanceRepository;
        IDertInfoUser _user;

        public ActivityService(
            IActivityMemberAttendanceRepository activityMemberAttendanceRepository,
            IActivityRepository activityRepository,
            IActivityTeamAttendanceRepository activityTeamAttendanceRepository,
            IDertInfoUser user
            )
        {
            _activityMemberAttendanceRepository = activityMemberAttendanceRepository;
            _activityRepository = activityRepository;
            _activityTeamAttendanceRepository = activityTeamAttendanceRepository;
            _user = user;
        }

        public async Task<IEnumerable<ActivityAttendanceDO>> GetActivityAttendances(int activityId)
        {
            var teamActivityAttendances = await _activityTeamAttendanceRepository.GetTeamAttendancesByActivityId(activityId);
            var memberActivityAttendances = await _activityMemberAttendanceRepository.GetMemberAttendancesByActivityId(activityId);

            var joined = new List<ActivityAttendanceDO>();
            joined.AddRange(teamActivityAttendances);
            joined.AddRange(memberActivityAttendances);

            return joined;
        }

        public async Task<IEnumerable<Activity>> GetDefaultsForEvent(int eventId, ActivityAudienceType audienceType)
        {
            var defaultActivities = await _activityRepository.GetDefaultsByEventAndAudience(eventId, audienceType);
            return defaultActivities;
        }

        public async Task<Activity> GetDetail(int activityId)
        {
            var activity = await _activityRepository.GetActivityDetail(activityId);
            return activity;
        }

        public async Task<Activity> UpdateActivity(Activity updatedActivity)
        {
            // Properties that are not permitted to be changed ever. 
            // - event binding

            // Properties that are not permitted to be changes based on event flow state
            // - price

            // Properties that are changed through other mechanisms
            // - isDefault

            var originalActivity = await _activityRepository.GetById(updatedActivity.Id);

            if (originalActivity == null) { throw new InvalidOperationException("Event Could Not Be Found"); }

            if (originalActivity.Title != updatedActivity.Title)
            {
                originalActivity.Title = updatedActivity.Title;
            }

            if (originalActivity.Description != updatedActivity.Description)
            {
                originalActivity.Description = updatedActivity.Description;
            }

            if (originalActivity.AudienceTypeId != updatedActivity.AudienceTypeId)
            {
                originalActivity.AudienceTypeId = updatedActivity.AudienceTypeId;
            }

            if (originalActivity.Price != updatedActivity.Price)
            {
                originalActivity.Price = updatedActivity.Price;
            }

            if (originalActivity.IsDefault != updatedActivity.IsDefault)
            {
                originalActivity.IsDefault = updatedActivity.IsDefault;
            }

            if (originalActivity.PriceTBC != updatedActivity.PriceTBC)
            {
                originalActivity.PriceTBC = updatedActivity.PriceTBC;
            }

            if (originalActivity.SoldOut != updatedActivity.SoldOut)
            {
                originalActivity.SoldOut = updatedActivity.SoldOut;
            }

            await _activityRepository.Update(originalActivity);

            return originalActivity;
        }
    }
}
