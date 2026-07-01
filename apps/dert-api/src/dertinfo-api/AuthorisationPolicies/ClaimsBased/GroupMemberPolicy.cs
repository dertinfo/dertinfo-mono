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
    public static class GroupMemberPolicy
    {
        public static string PolicyName { get { return "GroupMemberPolicy"; } }
    }

    public class GroupMemberRequirement : IClaimRequirement
    {
    }

    public class GroupMemberHandler : ClaimHandler<GroupMemberRequirement>
    {
        public GroupMemberHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupMemberRequirement requirement)
        {
            if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                var groupId = this.ExtractParameterFromContext("groupId", httpContext);
                var isThisGroupAdmin = this.TestClaim(context.User, "https://dertinfo.co.uk/groupadmin", groupId);
                var isThisGroupMember = this.TestClaim(context.User, "https://dertinfo.co.uk/groupmember", groupId);

                if (isThisGroupAdmin || isThisGroupMember) { context.Succeed(requirement); }
                // todo - We should also in the case of post requests be able to populate the groupId based on the posted object and the property of GroupId. 
                //      - this would simply a lot of the endpoints from a submission validation perspective.
            }

            return Task.CompletedTask;
        }

        
    }
}
