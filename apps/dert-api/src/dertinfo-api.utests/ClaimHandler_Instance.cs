using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Api.UTests
{

    /// <summary>
    /// Claim Handler is abstract so in order to test the functionality of the abstract we need to create an instance
    /// </summary>
    public class ClaimHandlerConcreteRequirement : IClaimRequirement
    {
    }

    // 
    /// <summary>
    /// Claim Handler is abstract so in order to test the functionality of the abstract class we need to create an instance.
    /// </summary>
    /// <remarks>
    /// We directly route the tests through to the abstract by hide the original 
    /// member with "new" and call them through the new public member
    /// </remarks>
    public class ClaimHandlerConcreteHandler : ClaimHandler<ClaimHandlerConcreteRequirement>
    {
        public ClaimHandlerConcreteHandler(IDertInfoConfiguration configuration) : base(configuration)
        {
        }

        public new bool ClaimExists(ClaimsPrincipal user, string claimType)
        {
            return base.ClaimExists(user, claimType);
        }

        public new bool TestClaim(ClaimsPrincipal user, string claimType, int claimValue)
        {
            return base.TestClaim(user, claimType, claimValue);
        }

        public new bool TestClaim(ClaimsPrincipal user, string claimType, bool claimValue)
        {
            return base.TestClaim(user, claimType, claimValue);
        }

        public new bool TestClaim(ClaimsPrincipal user, string claimType, string claimValue)
        {
            return base.TestClaim(user, claimType, claimValue);
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimHandlerConcreteRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}
