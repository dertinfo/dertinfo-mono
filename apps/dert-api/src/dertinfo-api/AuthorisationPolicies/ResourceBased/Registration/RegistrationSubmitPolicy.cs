using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationSubmitPolicy
    {
        public static string PolicyName { get { return "RegistrationSubmitPolicy"; } }
    }

    public class RegistrationSubmitRequirement : IClaimRequirement
    {
    }

    public class RegistrationSubmitHandler : ResourceClaimHandler<RegistrationSubmitRequirement, Registration>
    {
        public RegistrationSubmitHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationSubmitRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim)
            {
                context.Succeed(requirement);
            }

            if (hasGroupAdminClaimForRegistration)
            {
                // Allow Edits To Group Admin if is state new or submitted. 
                if (
                    resource.FlowState == RegistrationFlowState.New ||
                    resource.FlowState == RegistrationFlowState.Submitted
                    )
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
