using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public class PaperworkGetSignInSheetsPolicy
    {
        public static string PolicyName { get { return "PaperworkGetSignInSheetsPolicy"; } }
    }

    public class PaperworkGetSignInSheetsRequirement : IClaimRequirement
    {
    }

    public class PaperworkGetSignInSheetsHandler : ResourceClaimHandler<PaperworkGetSignInSheetsRequirement, Event>
    {
        public PaperworkGetSignInSheetsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PaperworkGetSignInSheetsRequirement requirement, Event resource)
        {
            var hasEventAdminClaimForEvent = this.TestClaim(context.User, "https://dertinfo.co.uk/eventadmin", resource.Id);
            var hasSuperAdminClaim = this.TestClaim(context.User, "https://dertinfo.co.uk/superadmin", true);

            if (hasSuperAdminClaim || hasEventAdminClaimForEvent)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
