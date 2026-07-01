using DertInfo.Api.AuthorisationPolicies.Base;
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
    public static class LookupJudgesPolicy
    {
        public static string PolicyName { get { return "LookupJudgesPolicy"; } }
    }

    public class LookupJudgesRequirement : IClaimRequirement
    {
    }

    public class LookupJudgesHandler : ResourceClaimHandler<LookupJudgesRequirement, Competition>
    {
        public LookupJudgesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LookupJudgesRequirement requirement, Competition resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasCompetitionAdminClaimForCompetition = this.TestClaim(context.User, "https://dertinfo.co.uk/competitionadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForEvent || hasCompetitionAdminClaimForCompetition)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
