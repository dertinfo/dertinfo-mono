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
    public static class InvoiceGetHistoryPolicy
    {
        public static string PolicyName { get { return "InvoiceGetHistoryPolicy"; } }
    }

    public class InvoiceGetHistoryRequirement : IClaimRequirement
    {
    }

    public class InvoiceGetHistoryHandler : ResourceClaimHandler<InvoiceGetHistoryRequirement, Invoice>
    {
        public InvoiceGetHistoryHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InvoiceGetHistoryRequirement requirement, Invoice resource)
        {
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.Registration.EventId);
            var hasGroupAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", resource.Registration.GroupId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForRegistration || hasGroupAdminClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
