using DertInfo.CrossCutting.Auth;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers.Base
{
    [Authorize]
    public class ResourceAuthController : AuthController
    {
        private readonly IAuthorizationService _authorizationService;


        public ResourceAuthController(
            IDertInfoUser user,
            IAuthorizationService authorizationService
            ): base(user)
        {
            _authorizationService = authorizationService;
        }

        protected async Task<bool> CheckAuthorisationPolicy(string policyName, object testObject)
        {
            Ensure.String.IsNotNullOrWhiteSpace(policyName);
            Ensure.Any.IsNotNull(testObject);

            var policyResult = await _authorizationService.AuthorizeAsync(User, testObject, policyName);
            return policyResult.Succeeded;
        }
    }
}
