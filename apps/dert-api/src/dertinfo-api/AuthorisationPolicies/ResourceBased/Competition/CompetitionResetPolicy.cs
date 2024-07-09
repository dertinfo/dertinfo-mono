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
    public static class CompetitionResetPolicy
    {
        public static string PolicyName { get { return "CompetitionResetPolicy"; } }
    }

    public class CompetitionResetRequirement : IClaimRequirement
    {
    }

    public class CompetitionResetHandler : ResourceClaimHandler<CompetitionResetRequirement, Competition>
    {
        public CompetitionResetHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompetitionResetRequirement requirement, Competition resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasCompetitionAdminClaimForCompetition = this.TestClaim(context.User, "https://dertinfo.co.uk/competitionadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForEvent || hasCompetitionAdminClaimForCompetition)
            {
                // If the compeition results are published then we are not permitted to reset it
                if (resource.FlowState != CompetitionFlowState.Published && !resource.ResultsPublished)
                {
                    // If we are in testing mode and in any other state then we are permitted to regenerate.
                    // or if we have not previously generated we are permitted
                    switch (resource.FlowState)
                    {
                        case CompetitionFlowState.New:
                        case CompetitionFlowState.Populated:
                            context.Succeed(requirement);
                            break;
                        case CompetitionFlowState.Generated:
                            if (resource.InTestingMode)
                            {
                                context.Succeed(requirement);
                            }
                            break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
