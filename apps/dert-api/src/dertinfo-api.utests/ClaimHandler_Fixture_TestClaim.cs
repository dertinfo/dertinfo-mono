using DertInfo.Api.AuthorisationPolicies.Base;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace DertInfo.Api.UTests
{
    public class ClaimHandler_Fixture_TestClaim : ClaimHandler_Setup
    {
        [Theory]
        [InlineData("CLAIM_TYPE", "false", "ISSUER_DOMAIN", false, true)]
        [InlineData("CLAIM_TYPE", "false", "ISSUER_DOMAIN", true, false)]
        [InlineData("CLAIM_TYPE", "true", "ISSUER_DOMAIN", false, false)]
        [InlineData("CLAIM_TYPE", "true", "ISSUER_DOMAIN", true, true)]
        [InlineData("https://dertinfo.co.uk/superadmin", "true", "dertinfo.eu.auth0.com", true, true)]
        [InlineData("https://dertinfo.co.uk/superadmin", "false", "dertinfotest.eu.auth0.com", false, true)]
        [InlineData("https://dertinfo.co.uk/gdprconsentgained", "false", "dertinfodev.eu.auth0.com", false, true)]
        public void Ensure_That_Boolean_Claim_Reports_Expected_Result(string claimType, string claimValue, string issuerDomain, bool compareValue, bool expectedResult)
        {
            // Arrange - Values
            var tokenIssuer = $"https://{issuerDomain}/";

            // Arrange - Environment
            Claim claim = data.ConstructClaim(claimType, claimValue, tokenIssuer);
            this.claimsPrincipal = data.ConstructClaimsPrincipalWithOneClaim(claim);

            // Arrange - Sut
            data.SetIssuerDomainInConfiguration(base.mockDertInfoConfiguration, issuerDomain);
            this.sut = data.ArrangeSut(base.mockDertInfoConfiguration); 

            // Act
            var result = this.sut.TestClaim(base.claimsPrincipal, claimType, compareValue);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("CLAIM_TYPE", "Bear", "ISSUER_DOMAIN", "Bear", true)]
        [InlineData("CLAIM_TYPE", "Cow", "ISSUER_DOMAIN", "Fish", false)]
        [InlineData("https://dertinfo.co.uk/privacy", "private", "dertinfo.eu.auth0.com", "public", false)]
        [InlineData("https://dertinfo.co.uk/marketing", "email", "dertinfotest.eu.auth0.com", "email", true)]
        public void Ensure_That_String_Claim_Reports_Expected_Result(string claimType, string claimValue, string issuerDomain, string compareValue, bool expectedResult)
        {
            // Arrange - Values
            var tokenIssuer = $"https://{issuerDomain}/";

            // Arrange - Environment
            Claim claim = data.ConstructClaim(claimType, claimValue, tokenIssuer);
            this.claimsPrincipal = data.ConstructClaimsPrincipalWithOneClaim(claim);

            // Arrange - Sut
            data.SetIssuerDomainInConfiguration(base.mockDertInfoConfiguration, issuerDomain);
            this.sut = data.ArrangeSut(base.mockDertInfoConfiguration);

            // Act
            var result = this.sut.TestClaim(base.claimsPrincipal, claimType, compareValue);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("CLAIM_TYPE", "1", "ISSUER_DOMAIN", 1, true)]
        [InlineData("CLAIM_TYPE", "2", "ISSUER_DOMAIN", 1, false)]
        [InlineData("https://dertinfo.co.uk/priority", "3", "dertinfo.eu.auth0.com", 2, false)]
        [InlineData("https://dertinfo.co.uk/membershiptype", "4", "dertinfotest.eu.auth0.com", 4, true)]
        public void Ensure_That_Integer_Claim_Reports_Expected_Result(string claimType, string claimValue, string issuerDomain, int compareValue, bool expectedResult)
        {
            // Arrange - Values
            var tokenIssuer = $"https://{issuerDomain}/";

            // Arrange - Environment
            Claim claim = data.ConstructClaim(claimType, claimValue, tokenIssuer);
            this.claimsPrincipal = data.ConstructClaimsPrincipalWithOneClaim(claim);

            // Arrange - Sut
            data.SetIssuerDomainInConfiguration(base.mockDertInfoConfiguration, issuerDomain);
            this.sut = data.ArrangeSut(base.mockDertInfoConfiguration);

            // Act
            var result = this.sut.TestClaim(base.claimsPrincipal, claimType, compareValue);

            // Assert
            result.ShouldBe(expectedResult);
        }
    }
}
