using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.API.Test.Database;

namespace MoneySaver.API.Test.SeedData
{
    public class BudgetContext : IAsyncLifetime
    {
        private readonly string userId = Guid.NewGuid().ToString();
        private MoneySaverApiContext dbContext;
        private BudgetService _budgetService;
        private IDateProvider _dateTimeProvider = new DateProvider();

        public async Task DisposeAsync()
        {
            await this.dbContext.Database.EnsureDeletedAsync();
        }

        public async Task InitializeAsync()
        {
            var mapper = this.GenerateMapperInstance();
            var userPackage = new UserPackage()
            {
                IsAdmin = true,
                UserId = this.userId
            };

            var dbSetup = new DatabaseInMemorySetup();

            this.dbContext = dbSetup.GetDatabase();

            var budgetRepository = MockingProviders.GetMockedRepository<Budget>(dbContext, userPackage);
            var budgetItemRepository = MockingProviders.GetMockedRepository<BudgetItem>(dbContext, userPackage);
            var transactionCategoryRepository = MockingProviders.GetMockedRepository<TransactionCategory>(dbContext, userPackage);
            var transactionRepository = MockingProviders.GetMockedRepository<Transaction>(dbContext, userPackage);

            this._budgetService = new BudgetService(
                budgetRepository,
                budgetItemRepository,
                transactionCategoryRepository,
                transactionRepository,
                mapper,
                MockingProviders.GetMockedLogger<BudgetService>().Object,
                userPackage,
                _dateTimeProvider
                );

            await SeedData();
        }

        public BudgetService GetBudgetService
            => this._budgetService;

        public async Task<Budget> GetBudgetEntityAsync(int id)
            => await this.dbContext.Set<Budget>().FirstAsync(x => x.Id == id);

        public IDateProvider GetDateProvider()
            => this._dateTimeProvider;

        private AutoMapper.Mapper GenerateMapperInstance()
        {
            var myProfile = new Api.Mapper();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new AutoMapper.Mapper(configuration);
        }

        private async Task SeedData()
        {
            var now = this._dateTimeProvider.GetDateTimeNow();
            var prevBudget = now.AddMonths(-1);
            var startMonth = new DateTime(prevBudget.Year, prevBudget.Month, 1);
            var endMonth = startMonth.AddMonths(1).AddTicks(-1);

            var cat1 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat1",
                    UserId = this.userId,
                });

            var cat2 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat2",
                    UserId = this.userId,
                });

            var cat3 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat2",
                    UserId = this.userId,
                });


            var dateBefore = startMonth.AddTicks(-1);

            await this.dbContext.Set<Budget>()
               .AddAsync(new Budget
               {
                   Name = "Budget test1",
                   StartDate = startMonth,
                   EndDate = endMonth,
                   BudgetType = BudgetType.Monthly,
                   UserId = this.userId,
                   IsInUse = true,
                   BudgetItems = new List<BudgetItem> {
                        new BudgetItem {
                        LimitAmount = 100,
                        TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                        UserId = this.userId
                        },
                        new BudgetItem {
                        LimitAmount = 200,
                        TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                        UserId = this.userId
                        },
                        new BudgetItem {
                        LimitAmount = 300,
                        TransactionCategoryId = cat3.Entity.TransactionCategoryId,
                        UserId = this.userId,
                        }
                   }
               });

            await this.dbContext.Set<Budget>()
              .AddAsync(new Budget
              {
                  Name = "Budget test2",
                  StartDate = new DateTime(dateBefore.Year, dateBefore.Month, 01),
                  EndDate = dateBefore,
                  BudgetType = BudgetType.Monthly,
                  UserId = this.userId,
                  BudgetItems = new List<BudgetItem> {
                        new BudgetItem {
                        LimitAmount = 100,
                        TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                        UserId = this.userId
                        },
                        new BudgetItem {
                        LimitAmount = 200,
                        TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                        UserId = this.userId
                        },
                        //new BudgetItem {
                        //LimitAmount = 300,
                        //TransactionCategoryId = cat3.Entity.TransactionCategoryId,
                        //UserId = this.userId
                        //}
                  }
              });

            await this.dbContext.Set<Transaction>()
                .AddAsync(new Transaction
                {
                    Amount = 10,
                    TransactionCategoryId = 1,
                    UserId = this.userId,
                    TransactionDate = DateTime.UtcNow.AddMonths(-1),
                });

            await this.dbContext.Set<Transaction>()
                .AddAsync(new Transaction
                {
                    Amount = 30,
                    TransactionCategoryId = 2,
                    UserId = this.userId,
                    TransactionDate = DateTime.UtcNow.AddMonths(-1),
                });

            await this.dbContext.SaveChangesAsync();
        }
    }
}
