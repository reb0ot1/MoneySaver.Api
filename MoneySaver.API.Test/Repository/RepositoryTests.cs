using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using Moq;

namespace MoneySaver.API.Test.Repository
{
    public class RepositoryTests
    {
        private readonly string userId = Guid.NewGuid().ToString();
        private Mock<ILogger<Repository<Budget>>> mockBudgetLogger = new Mock<ILogger<Repository<Budget>>>();
        private readonly UserPackage userPackage;

        public RepositoryTests()
        {
            this.userPackage = new UserPackage {
                IsAdmin = true,
                UserId = this.userId
            };
        }

        private async Task<MoneySaverApiContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<MoneySaverApiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new MoneySaverApiContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.EnsureDeletedAsync();
            if (dbContext.Budgets.Count() <= 0)
            { 
                var now = DateTime.UtcNow;
                var dayInMonth = DateTime.DaysInMonth(now.Year, now.Month);

                var budget = new Budget
                {
                    BudgetType = Api.Models.BudgetType.Monthly,
                    IsInUse = true,
                    StartDate = new DateTime(now.Year, now.Month, 1),
                    EndDate = new DateTime(now.Year, now.Month, dayInMonth).AddDays(1).AddTicks(-1),
                    BudgetItems = new List<BudgetItem>(),
                    Name = "Test",
                    UserId = this.userId
                };

                await dbContext.Budgets.AddAsync(budget);
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        [Fact]
        public async void MoneySaverRepository_GetAll_ReturnCollection()
        {
            //Arrange
            var context = await this.GetDbContext();
            var rep = new Repository<Budget>(context, this.userPackage, mockBudgetLogger.Object);
            //Act
            var collection = await rep.GetAll().ToListAsync();
            //Assert
            Assert.NotEmpty(collection);
        }
    }
}
