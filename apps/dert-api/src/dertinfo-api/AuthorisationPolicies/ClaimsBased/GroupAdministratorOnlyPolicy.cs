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
    public static class GroupAdministratorOnlyPolicy
    {
        public static string PolicyName { get { return "GroupAdministratorOnlyPolicy"; } }
    }

    public class GroupAdministratorOnlyRequirement : IClaimRequirement
    {
    }

    public class GroupAdministratorOnlyHandler : ClaimHandler<GroupAdministratorOnlyRequirement>
    {
        public GroupAdministratorOnlyHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupAdministratorOnlyRequirement requirement)
        {
            if (!this.ClaimExists(context.User, "https://dertinfo.co.uk/groupadmin")) { context.Fail(); }

            if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                var groupId = this.ExtractParameterFromContext("groupId", httpContext);
                var isGroupAdmin = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", groupId);
                if (isGroupAdmin) { context.Succeed(requirement); }
            }

            return Task.CompletedTask;
        }
    }
}
