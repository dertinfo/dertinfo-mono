using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationGetContactInfoPolicy
    {
        public static string PolicyName { get { return "RegistrationGetContactInfoPolicy"; } }
    }

    public class RegistrationGetContactInfoRequirement : IClaimRequirement
    {
    }

    public class RegistrationGetContactInfoHandler : ResourceClaimHandler<RegistrationGetContactInfoRequirement, Registration>
    {
        public RegistrationGetContactInfoHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationGetContactInfoRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim)
            {
                context.Succeed(requirement);
            }

            if (hasEventAdminClaimForRegistration || hasGroupAdminClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
