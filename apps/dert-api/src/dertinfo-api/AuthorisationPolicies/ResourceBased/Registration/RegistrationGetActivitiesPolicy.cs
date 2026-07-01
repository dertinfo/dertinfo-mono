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
    public class RegistrationGetActivitiesPolicy
    {
        public static string PolicyName { get { return "RegistrationGetActivitiesPolicy"; } }
    }

    public class RegistrationGetActivitiesRequirement : IClaimRequirement
    {
    }

    public class RegistrationGetActivitiesHandler : ResourceClaimHandler<RegistrationGetActivitiesRequirement, Registration>
    {
        public RegistrationGetActivitiesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationGetActivitiesRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForRegistration || hasGroupAdminClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
