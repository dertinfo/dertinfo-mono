using DertInfo.Api.AuthorisationPolicies.Base;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace DertInfo.Api.UTests
{
    public class ClaimHandler_Fixture_ClaimExisits : ClaimHandler_Setup
    {
        [Fact]
        public void Ensure_That_Claim_Exists_Reports_False_When_No_Claims()
        {
            // Arrange - Envionment
            this.claimsPrincipal = data.ConstructClaimsPrincipalWithNoClaims();
            var claimToLookFor = "A_MOCK_CLAIM";
            data.SetIssuerDomainInConfiguration(base.mockDertInfoConfiguration, "ISSUER_DOMAIN");

            // Arrange - Sut
            this.sut = data.ArrangeSut(base.mockDertInfoConfiguration);

            // Act
            var result = this.sut.ClaimExists(base.claimsPrincipal, claimToLookFor);

            // Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void Ensure_That_Claim_Exists_Reports_True_When_Claim_Present()
        {
            // Arrange - Variables
            var claimType = "CLAIM_TYPE";
            var issuerDomain = "ISSUER_DOMAIN";
            var tokenIssuer = $"https://{issuerDomain}/";
            var claimValue = "CLAIM_VALUE";

            // Arrange - Environment
            Claim claim = data.ConstructClaim(claimType, claimValue, tokenIssuer);
            this.claimsPrincipal = data.ConstructClaimsPrincipalWithOneClaim(claim);
            data.SetIssuerDomainInConfiguration(base.mockDertInfoConfiguration, issuerDomain);

            // Arrange - Sut
            this.sut = data.ArrangeSut(base.mockDertInfoConfiguration);

            // Act
            var result = this.sut.ClaimExists(base.claimsPrincipal, claimType);

            // Assert
            result.ShouldBe(true);
        }
    }
}
