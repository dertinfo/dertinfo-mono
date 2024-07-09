using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationViewTeamPolicy
    {
        public static string PolicyName { get { return "RegistrationViewTeamPolicy"; } }
    }

    public class RegistrationViewTeamRequirement : IClaimRequirement
    {
    }

    public class RegistrationViewTeamHandler : ResourceClaimHandler<RegistrationViewTeamRequirement, Registration>
    {
        public RegistrationViewTeamHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationViewTeamRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasGroupMemberClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupmember", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            // Group Member Claim allows group members to get the team attendances for a registration in order to allow them to view the results.
            if (hasSuperAdminClaim || hasGroupAdminClaimForRegistration  || hasEventAdminClaimForRegistration || hasGroupMemberClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
