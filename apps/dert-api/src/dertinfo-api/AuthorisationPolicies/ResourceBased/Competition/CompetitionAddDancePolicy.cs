using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.Api.AuthorisationPolicies.ResourceBased.Tests;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public static class CompetitionAddDancePolicy
    {
        public static string PolicyName { get { return "CompetitionAddDancePolicy"; } }
    }

    public class CompetitionAddDanceRequirement : IClaimRequirement
    {
    }

    public class CompetitionAddDanceHandler : ResourceClaimHandler<CompetitionAddDanceRequirement, Competition>
    {
        public CompetitionAddDanceHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompetitionAddDanceRequirement requirement, Competition resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasCompetitionAdminClaimForCompetition = this.TestClaim(context.User, "https://dertinfo.co.uk/competitionadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (CompetitionRules.AddDancePermitted(resource) && (hasSuperAdminClaim || hasEventAdminClaimForEvent || hasCompetitionAdminClaimForCompetition))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
