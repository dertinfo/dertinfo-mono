using DertInfo.CrossCutting.Configuration;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.Base
{
    public interface IClaimRequirement : IAuthorizationRequirement
    {
    }

    public abstract class ClaimHandler<T> : AuthorizationHandler<T> where T : IClaimRequirement
    {
        private readonly string _issuer;

        protected ClaimHandler(IDertInfoConfiguration configuration)
        {
            Ensure.String.IsNotNullOrEmpty(configuration.Auth0_Domain);

            _issuer = $"https://{configuration.Auth0_Domain}/";
        }

        protected bool ClaimExists(ClaimsPrincipal user, string claimType)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer);
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, int claimValue) {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && int.Parse(c.Value) == claimValue);
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, bool claimValue)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && bool.Parse(c.Value) == claimValue);
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, string claimValue)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && c.Value == claimValue);
        }

        protected string ExtractParameterFromContext(string parameterName, DefaultHttpContext httpContext)
        {
            return httpContext.GetRouteData().Values[parameterName] as string;
        }

    }

    public abstract class ResourceClaimHandler<T, R> : AuthorizationHandler<T, R> where T : IClaimRequirement
    {
        protected readonly string _issuer;

        protected ResourceClaimHandler(IDertInfoConfiguration configuration)
        {
            Ensure.String.IsNotNullOrEmpty(configuration.Auth0_Domain);

            _issuer = $"https://{configuration.Auth0_Domain}/";
        }

        protected bool ClaimExists(ClaimsPrincipal user, string claimType)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer);
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, int claimValue)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && c.Value == claimValue.ToString());
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, bool claimValue)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && c.Value == claimValue.ToString());
        }

        protected bool TestClaim(ClaimsPrincipal user, string claimType, string claimValue)
        {
            return user.HasClaim(c => c.Type == claimType && c.Issuer == _issuer && c.Value == claimValue.ToString());
        }
    }
}
