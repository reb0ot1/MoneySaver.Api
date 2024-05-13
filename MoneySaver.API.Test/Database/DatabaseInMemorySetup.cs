using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;

namespace MoneySaver.API.Test.Database
{
    public class DatabaseInMemorySetup
    {
        private MoneySaverApiContext dbContext;

        public DatabaseInMemorySetup()
        {
            var options = new DbContextOptionsBuilder<MoneySaverApiContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

            this.dbContext = new MoneySaverApiContext(options);
        }

        public MoneySaverApiContext GetDatabase()
        {
            return this.dbContext;
        }
    }
}
