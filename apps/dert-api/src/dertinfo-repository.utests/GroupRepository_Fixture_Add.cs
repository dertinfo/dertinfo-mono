using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DertInfo.Repository.UTests
{
    public class GroupRepository_Fixture_Add: GroupRepository_Setup
    {
        [Fact]
        public async void Ensure_That_In_Memory_Context_Reports_Empty()
        {
            // Arrange + Act
            var dbSet = base.mockMemoryContext.Groups;
            var hasEntities = await dbSet.AnyAsync();

            // Assert
            hasEntities.ShouldBe(false);
        }

        [Fact]
        public async void Add_Writes_Single_To_DbSet()
        {
            // Arrange + Act
            var group = await base.sut.Add(new Group());
            var dbSet = base.mockMemoryContext.Groups;
            var hasEntities = await dbSet.AnyAsync();

            // Assert
            hasEntities.ShouldBe(true);
        }
    }
}
