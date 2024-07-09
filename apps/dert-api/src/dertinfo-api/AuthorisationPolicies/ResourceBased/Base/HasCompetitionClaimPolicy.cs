using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased.Base
{
    public abstract class HasCompetitionClaimPolicy
    {
        protected HasCompetitionClaimPolicy()
        {
        }

        public static string PolicyName { get { return "HasCompetitionClaimPolicy"; } }
    }

    public abstract class HasCompetitionClaimRequirement : IClaimRequirement
    {
    }

    public abstract class HasCompetitionClaimHandler : ResourceClaimHandler<HasCompetitionClaimRequirement, Competition>
    {
        protected HasCompetitionClaimHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasCompetitionClaimRequirement requirement, Competition resource)
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
