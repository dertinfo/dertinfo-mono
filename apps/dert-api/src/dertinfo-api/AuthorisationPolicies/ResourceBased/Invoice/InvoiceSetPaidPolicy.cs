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
    public static class InvoiceSetPaidPolicy
    {
        public static string PolicyName { get { return "InvoiceSetPaidPolicy"; } }
    }

    public class InvoiceSetPaidRequirement : IClaimRequirement
    {
    }

    public class InvoiceSetPaidHandler : ResourceClaimHandler<InvoiceSetPaidRequirement, Invoice>
    {
        public InvoiceSetPaidHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InvoiceSetPaidRequirement requirement, Invoice resource)
        {
            var hasEventAdminClaimForRegistration = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.Registration.EventId);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForRegistration)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
