using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MoneySaver.Api.Data.Postgres
{
    // Used by `dotnet ef` only; the running app configures the context through MoneySaver.System.
    public class MoneySaverApiContextFactory : IDesignTimeDbContextFactory<MoneySaverApiContext>
    {
        public MoneySaverApiContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Port=9432;Database=moneysaver;Username=moneysaver;Password=postgres";

            var options = new DbContextOptionsBuilder<MoneySaverApiContext>()
                .UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsAssembly(typeof(MoneySaverApiContextFactory).Assembly.GetName().Name))
                .Options;

            return new MoneySaverApiContext(options);
        }
    }
}
