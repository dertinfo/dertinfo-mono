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
    public static class DodAdministratorOnlyPolicy
    {
        public static string PolicyName { get { return "DodAdministratorOnlyPolicy"; } }
    }

    public class DodAdministratorOnlyRequirement : IClaimRequirement
    {
    }

    public class DodAdministratorOnlyHandler : ClaimHandler<DodAdministratorOnlyRequirement>
    {
        public DodAdministratorOnlyHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DodAdministratorOnlyRequirement requirement)
        {
            if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);
                var hasDodAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/dodadmin", true);

                if (hasSuperAdminClaim || hasDodAdminClaim)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
