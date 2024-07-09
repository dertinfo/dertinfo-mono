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
    public static class EventAdministratorOnlyPolicy
    {
        public static string PolicyName { get { return "EventAdministratorOnlyPolicy"; } }
    }

    public class EventAdministratorOnlyRequirement : IClaimRequirement
    {
    }

    public class EventAdministratorOnlyHandler : ClaimHandler<EventAdministratorOnlyRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventAdministratorOnlyHandler(IHttpContextAccessor httpContextAccessor, IDertInfoConfiguration configuration) : base(configuration)
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EventAdministratorOnlyRequirement requirement)
        {
            if (!ClaimExists(context.User, "https://dertinfo.co.uk/eventadmin")) { context.Fail(); }

            if (context.Resource is Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                var eventId = this.ExtractParameterFromContext("eventId", httpContext);
                var isThisEventAdmin = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", eventId);

                if (isThisEventAdmin)
                {
                    context.Succeed(requirement);
                }
            }

            // Reject Baseline
            return Task.CompletedTask;
        }
    }
}
