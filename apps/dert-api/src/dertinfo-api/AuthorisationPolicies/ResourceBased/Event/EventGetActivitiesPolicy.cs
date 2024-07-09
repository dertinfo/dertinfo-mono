using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class EventGetActivitiesPolicy
    {
        public static string PolicyName { get { return "EventGetActivitiesPolicy"; } }
    }

    public class EventGetActivitiesRequirement : IClaimRequirement
    {
    }

    public class EventGetActivitiesHandler : ResourceClaimHandler<EventGetActivitiesRequirement, Event>
    {
        public EventGetActivitiesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EventGetActivitiesRequirement requirement, Event resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForEvent)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
