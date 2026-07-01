using Auth0.ManagementApi.Models;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.System;
using DertInfo.Services.ExternalProviders;
using Microsoft.ApplicationInsights;
using System;

namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiClient_Setup_Data
    {
        internal UserAccessClaims BuildUserAccessClaimsToAddOrRemove(string[] groupAdminClaims, string[] eventAdminClaims, string[] venueAdminClaims, string[] groupMemberClaims)
        {
            var userAccessClaims = new UserAccessClaims()
            {
                GroupPermissions = groupAdminClaims,
                EventPermissions = eventAdminClaims,
                VenuePermissions = venueAdminClaims,
                GroupMemberPermissions = groupMemberClaims
            };

            return userAccessClaims;
        }

        internal User BuildAuth0UserWithClaims(string[] groupAdminClaims, string[] eventAdminClaims, string[] venueAdminClaims, string[] groupMemberClaims)
        {
            return new Auth0.ManagementApi.Models.User()
            {
                AppMetadata = BuildAppMetaDataDynamic(groupAdminClaims, eventAdminClaims, venueAdminClaims, groupMemberClaims)
            };
        }

        private dynamic BuildAppMetaDataDynamic(string[] groupAdminClaims, string[] eventAdminClaims, string[] venueAdminClaims, string[] groupMemberClaims)
        {
            dynamic myDynamic = new System.Dynamic.ExpandoObject();

            myDynamic.groupadmin = groupAdminClaims;
            myDynamic.eventadmin = eventAdminClaims;
            myDynamic.venueadmin = venueAdminClaims;
            myDynamic.groupmember = groupMemberClaims;
            myDynamic.ToObject = new Func<UserAppMetaData>(() =>
            {
                return new UserAppMetaData()
                {
                    groupadmin = groupAdminClaims,
                    eventadmin = eventAdminClaims,
                    venueadmin = venueAdminClaims,
                    groupmember = groupMemberClaims
                };
            });

            return myDynamic;
        }
    }
}
