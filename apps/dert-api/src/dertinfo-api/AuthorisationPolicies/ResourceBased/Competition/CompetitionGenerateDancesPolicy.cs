using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public static class CompetitionGenerateDancesPolicy
    {
        public static string PolicyName { get { return "CompetitionGenerateDancesPolicy"; } }
    }

    public class CompetitionGenerateDancesRequirement : IClaimRequirement
    {
    }

    public class CompetitionGenerateDancesHandler : ResourceClaimHandler<CompetitionGenerateDancesRequirement, Competition>
    {
        public CompetitionGenerateDancesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompetitionGenerateDancesRequirement requirement, Competition resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasCompetitionAdminClaimForCompetition = this.TestClaim(context.User, "https://dertinfo.co.uk/competitionadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForEvent || hasCompetitionAdminClaimForCompetition)
            {
                // If the compeition results are published then we are not permitted to GenerateDances it
                if (resource.FlowState != CompetitionFlowState.Published || !resource.ResultsPublished)
                {
                    // If we are in testing mode and in any other state then we are permitted to regenerate.
                    // or if we have not previously generated we are permitted
                    if (resource.FlowState != CompetitionFlowState.Generated && resource.HasBeenPopulated)
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
