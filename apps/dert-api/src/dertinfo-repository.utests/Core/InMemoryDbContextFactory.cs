using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DertInfo.Repository.UTests.Core
{
    /// <summary>
    /// This is used to help build a database context for the purpose of the unit tests
    /// </summary>
    public class InMemoryDbContextFactory
    {
        public DertInfoContext GetPersistantDertInfoContext()
        {
            var dbOptions = new DbContextOptionsBuilder<DertInfoContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryDertInfoDatabase")
                            .Options;

            var dbContext = new DertInfoContext(dbOptions);

            return dbContext;
        }

        public DertInfoContext GetNewInstanceDertInfoContext()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DertInfoContext>();
            builder.UseInMemoryDatabase(databaseName: "InMemoryDertInfoDatabase")
                   .UseInternalServiceProvider(serviceProvider);

            var dbContext = new DertInfoContext(builder.Options);

            return dbContext;
        }
    }
}
