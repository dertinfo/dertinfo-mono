using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository.UTests.Core;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Repository.UTests
{
    public class GroupRepository_Setup
    {
        protected GroupRepository sut;
        protected GroupRepository_Setup_Data data;

        protected DertInfoContext mockMemoryContext;
        protected IDertInfoUser mockDertInfoUser;

        public GroupRepository_Setup()
        {
            // Construct an in memory context
            this.mockMemoryContext = new InMemoryDbContextFactory().GetNewInstanceDertInfoContext();

            // Construct other mocks
            this.mockDertInfoUser = Substitute.For<IDertInfoUser>();

            // Create the data service
            this.data = new GroupRepository_Setup_Data();

            // Build the sut
            this.sut = new GroupRepository(mockMemoryContext, mockDertInfoUser);
        }
    }
}
