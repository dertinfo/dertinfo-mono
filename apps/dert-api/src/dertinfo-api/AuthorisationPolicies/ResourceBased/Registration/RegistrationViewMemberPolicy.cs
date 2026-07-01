using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationViewMemberPolicy
    {
        public static string PolicyName { get { return "RegistrationViewMemberPolicy"; } }
    }

    public class RegistrationViewMemberRequirement : IClaimRequirement
    {
    }

    public class RegistrationViewMemberHandler : ResourceClaimHandler<RegistrationViewMemberRequirement, Registration>
    {
        public RegistrationViewMemberHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationViewMemberRequirement requirement, Registration resource)
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
