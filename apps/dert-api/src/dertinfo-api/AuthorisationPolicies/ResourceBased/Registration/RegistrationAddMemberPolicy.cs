using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public static class RegistrationAddMemberPolicy
    {
        public static string PolicyName { get { return "RegistrationAddMemberPolicy"; } }
    }

    public class RegistrationAddMemberRequirement : IClaimRequirement
    {
    }

    public class RegistrationAddMemberHandler : ResourceClaimHandler<RegistrationAddMemberRequirement, Registration>
    {
        public RegistrationAddMemberHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RegistrationAddMemberRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", resource.EventId);

            if (hasSuperAdminClaim)
            {
                context.Succeed(requirement);
            }

            if (hasEventAdminClaimForRegistration)
            {
                // Allow Edits To Event Admin                
                if (
                    resource.FlowState == RegistrationFlowState.New ||
                    resource.FlowState == RegistrationFlowState.Submitted ||
                    resource.FlowState == RegistrationFlowState.Confirmed 
                    )
                {
                    context.Succeed(requirement);
                }
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
