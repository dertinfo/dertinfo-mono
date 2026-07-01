using DertInfo.Api.AuthorisationPolicies.Base;
using DertInfo.CrossCutting.Configuration;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Api.UTests
{
    public class ClaimHandler_Setup
    {
        protected ClaimHandlerConcreteHandler sut;
        protected ClaimHandlerConcreteRequirement testRequirement;
        protected ClaimHandler_Setup_Data data;

        protected ClaimsPrincipal claimsPrincipal;
        protected IDertInfoConfiguration mockDertInfoConfiguration;

        public ClaimHandler_Setup()
        {
            // Construct the dependencies
            this.mockDertInfoConfiguration = Substitute.For<IDertInfoConfiguration>();

            // Create the data service
            this.data = new ClaimHandler_Setup_Data();

            /*
             * In many cases where the sut doesn't change we might build the sut here.
             * However in this case we want to manipulate the configuration and the 
             * construction of that sut depends on the configuration so we defer the creation of
             * the sut to the "Arrange" part of the unit tests.
             */
        }
    }

    
}
