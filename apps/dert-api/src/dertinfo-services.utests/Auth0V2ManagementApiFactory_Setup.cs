using DertInfo.CrossCutting.Configuration;
using DertInfo.Services.ExternalProviders;
using Microsoft.ApplicationInsights;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiFactory_Setup
    {
        protected Auth0V2ManagementApiFactory sut;
        protected Auth0V2ManagementApiFactory_Setup_Data data;

        protected IAuth0V2ManagementApiTokenRequester mockTokenRequester;
        protected IDertInfoConfiguration mockDertInfoConfiguration;

        public Auth0V2ManagementApiFactory_Setup()
        {
            // Construct the dependencies
            this.mockDertInfoConfiguration = Substitute.For<IDertInfoConfiguration>();
            this.mockTokenRequester = Substitute.For<IAuth0V2ManagementApiTokenRequester>();

            // Create the data service
            this.data = new Auth0V2ManagementApiFactory_Setup_Data();

        }
    }
}
