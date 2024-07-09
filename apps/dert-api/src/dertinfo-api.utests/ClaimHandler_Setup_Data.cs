using DertInfo.CrossCutting.Configuration;
using NSubstitute;
using System.Collections.Generic;
using System.Security.Claims;

namespace DertInfo.Api.UTests
{
    public class ClaimHandler_Setup_Data
    {
        public ClaimHandler_Setup_Data() 
        { 
        }

        public ClaimHandlerConcreteHandler ArrangeSut(IDertInfoConfiguration configuration)
        {
            return new ClaimHandlerConcreteHandler(configuration);
        }

        public ClaimsPrincipal ConstructClaimsPrincipalWithNoClaims()
        {
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        public ClaimsPrincipal ConstructClaimsPrincipalWithOneClaim(Claim claim)
        {
            var claims = new List<Claim>() { claim };
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        public Claim ConstructClaim(string claimType, string claimValue, string claimIssuer)
        {
            return new Claim(claimType, claimValue, null, claimIssuer);
        }

        public void SetIssuerDomainInConfiguration(IDertInfoConfiguration mockDertInfoConfiguration, string claimIssuerDomain)
        {
            mockDertInfoConfiguration.Auth0_Domain.Returns(claimIssuerDomain);
        }

        
    }
}
