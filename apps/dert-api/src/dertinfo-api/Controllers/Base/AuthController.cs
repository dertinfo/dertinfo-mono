using DertInfo.CrossCutting.Auth;
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
    public class AuthController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        protected IDertInfoUser _user;

        public AuthController(
            IDertInfoUser user
            )
        {
            _user = user;
        }

        protected void ExtractUser()
        {
            _user.AuthId = User.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;
            _user.ClaimsCompetitionAdmin = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/competitionadmin").Select(cl => cl.Value).ToList();
            _user.ClaimsEventAdmin = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/eventadmin").Select(cl => cl.Value).ToList();
            _user.ClaimsGroupAdmin = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/groupadmin").Select(cl => cl.Value).ToList();
            _user.ClaimsGroupMember = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/groupmember").Select(cl => cl.Value).ToList();
            _user.ClaimsVenueAdmin = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/venueadmin").Select(cl => cl.Value).ToList();

            var isSuperAdminStr = User.Claims.Where(c => c.Type == "https://dertinfo.co.uk/superadmin").FirstOrDefault();
            _user.IsSuperAdmin = isSuperAdminStr != null ? bool.Parse(isSuperAdminStr.Value) : false;
        }
    }
}
