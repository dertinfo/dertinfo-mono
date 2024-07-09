using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ClaimsBased
{
    public static class SuperAdministratorOnlyPolicy
    {
        public static string PolicyName { get { return "SuperAdministratorOnlyPolicy"; } }
    }

    public class SuperAdministratorOnlyRequirement : IClaimRequirement
    {
    }

    public class SuperAdministratorOnlyHandler : ClaimHandler<SuperAdministratorOnlyRequirement>
    {

        public SuperAdministratorOnlyHandler(IDertInfoConfiguration configuration): base(configuration)
        {}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdministratorOnlyRequirement requirement)
        {
            if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);
                if (hasSuperAdminClaim){context.Succeed(requirement);}
            }

            return Task.CompletedTask;
        }
    }
}
