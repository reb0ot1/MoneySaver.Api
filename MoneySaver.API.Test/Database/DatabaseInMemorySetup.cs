using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;

namespace MoneySaver.API.Test.Database
{
    public class DatabaseInMemorySetup
    {
        private MoneySaverApiContext dbContext;

        private SqliteConnection _connection;
        
        public DatabaseInMemorySetup()
        {
            this._connection = new SqliteConnection($"Data Source={Guid.NewGuid()}.db");
            this._connection.Open();
            var options = new DbContextOptionsBuilder<MoneySaverApiContext>()
                        .UseSqlite(this._connection)
                        .Options;
            
            this.dbContext = new MoneySaverApiContext(options);
        }

        public MoneySaverApiContext GetDatabase()
        {
            return this.dbContext;
        }
        
        public Task RemoveAsync()
        {
            return this._connection.CloseAsync();
        }
    }
}
