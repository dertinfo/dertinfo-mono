using Auth0.ManagementApi;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.System;
using DertInfo.Services.ExternalProviders;
using NSubstitute;


namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiClient_Setup
    {
        protected Auth0V2ManagementApiClient sut;
        protected Auth0V2ManagementApiClient_Setup_Data data;
        protected IDertInfoConfiguration mockDertInfoConfiguration;
        protected IAuth0V2ManagementApiFactory mockManagementApiFactory;
        protected IAuth0V2ManagementApiFacade mockManagementApiFacade;

        public Auth0V2ManagementApiClient_Setup()
        {
            // Construct the dependencies
            this.mockDertInfoConfiguration = Substitute.For<IDertInfoConfiguration>();
            this.mockManagementApiFactory = Substitute.For<IAuth0V2ManagementApiFactory>();
            this.mockManagementApiFacade = Substitute.For<IAuth0V2ManagementApiFacade>();

            // Create the data service
            this.data = new Auth0V2ManagementApiClient_Setup_Data();

            // Create the sut
            this.sut = new Auth0V2ManagementApiClient(mockManagementApiFactory);
        }

    }
}
