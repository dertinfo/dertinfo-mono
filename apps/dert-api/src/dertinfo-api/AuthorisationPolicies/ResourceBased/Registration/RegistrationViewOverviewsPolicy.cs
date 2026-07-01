using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationViewOverviewsPolicy
    {
        public static string PolicyName { get { return "RegistrationViewOverviewsPolicy"; } }
    }

    public class RegistrationViewOverviewsRequirement : IClaimRequirement
    {
    }

    public class RegistrationViewOverviewsHandler : ResourceClaimHandler<RegistrationViewOverviewsRequirement, Registration>
    {
        public RegistrationViewOverviewsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationViewOverviewsRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim)
            {
                context.Succeed(requirement);
            }

            if (hasGroupAdminClaimForRegistration || hasEventAdminClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
