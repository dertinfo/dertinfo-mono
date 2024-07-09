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
    /// Change activity policy is based on the flowstate of the registration.
    /// </summary>
    public class AttendanceChangeActivitiesPolicy
    {
        public static string PolicyName { get { return "AttendanceChangeActivitiesPolicy"; } }
    }

    public class AttendanceChangeActivitiesRequirement : IClaimRequirement
    {
    }

    public class AttendanceChangeActivitiesHandler : ResourceClaimHandler<AttendanceChangeActivitiesRequirement, Registration>
    {
        public AttendanceChangeActivitiesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AttendanceChangeActivitiesRequirement requirement, Registration resource)
        {
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.GroupId);
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

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

            // todo - group member could also make changes here.

            return Task.CompletedTask;
        }
    }
}
