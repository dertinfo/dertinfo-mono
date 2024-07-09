using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class TeamGetResultsPolicy
    {
        public static string PolicyName { get { return "TeamGetResultsPolicy"; } }
    }

    public class TeamGetResultsRequirement : IClaimRequirement
    {
    }

    public class TeamGetResultsHandler : ResourceClaimHandler<TeamGetResultsRequirement, Team>
    {
        public TeamGetResultsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamGetResultsRequirement requirement, Team resource)
        {
            var hasGroupAdminClaimForTeam = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasGroupMemberClaimForTeam = this.TestClaim(context.User, "https://dertinfo.co.uk/groupmember", resource.GroupId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasGroupAdminClaimForTeam || hasGroupMemberClaimForTeam)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
