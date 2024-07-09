using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using DertInfo.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.UTests
{
    public class VenueService_Setup
    {
        protected VenueService sut;
        protected VenueService_Setup_Data data;

        protected IVenueRepository mockVenueRepo;
        protected IDertInfoUser mockDertInfoUser;


        public VenueService_Setup()
        {
            // Construct the dependencies
            this.mockDertInfoUser = Substitute.For<IDertInfoUser>();
            this.mockVenueRepo = Substitute.For<IVenueRepository>();

            // Create the data service
            this.data = new VenueService_Setup_Data();

            // Build the sut
            this.sut = new VenueService(mockVenueRepo, mockDertInfoUser);
        }
    }
}
