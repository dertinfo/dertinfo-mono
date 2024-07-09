using DertInfo.Api.ITests.Core;
using DertInfo.Models.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DertInfo.Api.ITests
{
    public class StatusController_Fixture_CheckStatus : IntergrationTestBase<GroupDto>
    {
        public StatusController_Fixture_CheckStatus() : base()
        {

        }

        [Fact]
        public async Task Check_Status_Controller_Returns_Response()
        {
            if (this._client == null && this._server == null)
            {
                await base.InitialiseApplicationHostAndClient();
            }

            // Arrange
            var statusUrl = "/api/status";

            // Act
            var response = await _client.GetAsync(statusUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
