using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.API.Test.Database;

namespace MoneySaver.API.Test.SeedData
{
    public class CategoryContext : IAsyncLifetime
    {
        private TransactionCategoryService _sut;
        private MoneySaverApiContext dbContext;
        private UserPackage userPackage = new UserPackage { UserId = Guid.NewGuid().ToString(), IsAdmin = true };

        public async Task DisposeAsync()
        {
            await this.dbContext.Database.EnsureDeletedAsync();
        }

        public async Task InitializeAsync()
        {
            var dbSetup = new DatabaseInMemorySetup();
            this.dbContext = dbSetup.GetDatabase();
            await this.dbContext.Database.EnsureCreatedAsync();

            this._sut = new TransactionCategoryService(
                    MockingProviders.GetMockedLogger<TransactionCategoryService>().Object,
                    MockingProviders.GetMockedRepository<TransactionCategory>(this.dbContext, this.userPackage),
                    MockingProviders.GenerateMapperInstance(),
                    this.userPackage
                );

            await this.SeedDataAsync();
        }

        public TransactionCategoryService GetService()
            => this._sut;

        public async Task<TransactionCategory> GetCategoryEntityAsync(int id)
            => await this.dbContext.Set<TransactionCategory>().FirstAsync(x => x.TransactionCategoryId == id);

        private async Task SeedDataAsync()
        {
            var cat1 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "TestCat1",
                    UserId = this.userPackage.UserId
                });

            await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "TestCat2",
                    UserId = this.userPackage.UserId,
                    IsDeleted = true
                });

            await this.dbContext.AddAsync<TransactionCategory>(
               new TransactionCategory
               {
                   Name = "TestCat3",
                   UserId = this.userPackage.UserId,
                   ParentId = cat1.Entity.TransactionCategoryId
               });

            await this.dbContext.AddAsync<TransactionCategory>(
               new TransactionCategory
               {
                   Name = "TestCat4",
                   UserId = this.userPackage.UserId
               });

            await this.dbContext.SaveChangesAsync();
        }
    }
}
