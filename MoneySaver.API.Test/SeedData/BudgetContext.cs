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
        private DatabaseInMemorySetup _dbInMemory;
        private MoneySaverApiContext dbContext;
        private BudgetService _budgetService;
        private TransactionCategoryService _transactionCategoryService;
        private BudgetItemService _budgetItemsService;
        private TransactionService _transactionService;
        private IDateProvider _dateTimeProvider = new DateProvider();
        private UserPackage userPackage = new UserPackage { UserId = Guid.NewGuid().ToString(), IsAdmin = true };

        public async Task DisposeAsync()
        {
            await this._dbInMemory.RemoveAsync();
        }

        public async Task InitializeAsync()
        {
            this._dbInMemory = new DatabaseInMemorySetup();
            this.dbContext = this._dbInMemory.GetDatabase();
            await this.dbContext.Database.EnsureCreatedAsync();

            var mapper = MockingProviders.GenerateMapperInstance();
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

            this._transactionCategoryService = new TransactionCategoryService(
                MockingProviders.GetMockedLogger<TransactionCategoryService>().Object,
                transactionCategoryRepository,
                mapper,
                userPackage
            );

            this._budgetItemsService = new BudgetItemService(
                MockingProviders.GetMockedLogger<BudgetItemService>().Object,
                budgetItemRepository,
                transactionCategoryRepository,
                transactionRepository,
                budgetRepository
            );
            
            this._transactionService = new TransactionService(
                transactionRepository,
                transactionCategoryRepository,
                mapper,
                MockingProviders.GetMockedLogger<TransactionService>().Object,
                userPackage
            );

        }
        
        public BudgetService GetBudgetService
            => this._budgetService;
        
        public TransactionCategoryService GetTransactionCategoryService
            => this._transactionCategoryService;
        
        public BudgetItemService GetBudgetItemsService
            => this._budgetItemsService;
        
        public TransactionService GetTransactionService
            => this._transactionService;

        public async Task<Budget> GetBudgetEntityAsync(int id)
            => await this.dbContext.Set<Budget>().FirstAsync(x => x.Id == id);

        public IDateProvider GetDateProvider()
            => this._dateTimeProvider;

        public async Task ClearDataAsync()
        {
            await this.dbContext.Set<Transaction>().Where(e => true).ExecuteDeleteAsync();
            await this.dbContext.Set<Budget>().Where(e => true).ExecuteDeleteAsync();
            await this.dbContext.Set<TransactionCategory>().Where(e => true).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<TransactionCategory>> SeedCategories(IEnumerable<TransactionCategory> categories)
        {
            var result = new List<TransactionCategory>();
            foreach (var category in categories)
            {
                var categoryCreated = await this.dbContext.AddAsync<TransactionCategory>(category);
                category.TransactionCategoryId = categoryCreated.Entity.TransactionCategoryId;
                result.Add(category);
            }
            
            await this.dbContext.SaveChangesAsync();

            return result;
        }
        
        public async Task<IEnumerable<TransactionCategory>> SeedBudgets(IEnumerable<TransactionCategory> categories)
        {
            var result = new List<TransactionCategory>();
            foreach (var category in categories)
            {
                var categoryCreated = await this.dbContext.AddAsync<TransactionCategory>(category);
                category.TransactionCategoryId = categoryCreated.Entity.TransactionCategoryId;
                result.Add(category);
            }
            
            await this.dbContext.SaveChangesAsync();

            return result;
        }

        public async Task SeedData()
        {
            var now = this._dateTimeProvider.GetDateTimeNow();
            var prevBudget = now.AddMonths(-1);
            var startMonth = new DateTime(prevBudget.Year, prevBudget.Month, 1);
            var endMonth = startMonth.AddMonths(1).AddTicks(-1);
            
            await this.dbContext.Database.EnsureCreatedAsync();
            var cat1 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat1",
                    UserId = this.userPackage.UserId,
                });

            var cat2 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat2",
                    UserId = this.userPackage.UserId,
                });

            var cat3 = await this.dbContext.AddAsync<TransactionCategory>(
                new TransactionCategory
                {
                    Name = "Cat2",
                    UserId = this.userPackage.UserId,
                });

            await this.dbContext.SaveChangesAsync();
            
            var dateBefore = startMonth.AddTicks(-1);
            
            await this.dbContext.Set<Budget>()
               .AddAsync(new Budget
               {
                   Name = "Budget test1",
                   StartDate = startMonth,
                   EndDate = endMonth,
                   BudgetType = BudgetType.Monthly,
                   UserId = this.userPackage.UserId,
                   IsInUse = true,
                   BudgetItems = new List<BudgetItem> {
                        new BudgetItem {
                        LimitAmount = 100,
                        TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                        UserId = this.userPackage.UserId
                        },
                        new BudgetItem {
                        LimitAmount = 200,
                        TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                        UserId = this.userPackage.UserId
                        },
                        new BudgetItem {
                        LimitAmount = 300,
                        TransactionCategoryId = cat3.Entity.TransactionCategoryId,
                        UserId = this.userPackage.UserId,
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
                  UserId = this.userPackage.UserId,
                  BudgetItems = new List<BudgetItem> {
                        new BudgetItem {
                        LimitAmount = 100,
                        TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                        UserId = this.userPackage.UserId
                        },
                        new BudgetItem {
                        LimitAmount = 200,
                        TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                        UserId = this.userPackage.UserId
                        }
                  }
              });
            await this.dbContext.SaveChangesAsync();
            
            await this.dbContext.Set<Transaction>()
                .AddAsync(new Transaction
                {
                    Amount = 10,
                    TransactionCategoryId = cat1.Entity.TransactionCategoryId,
                    UserId = this.userPackage.UserId,
                    TransactionDate = DateTime.UtcNow.AddMonths(-1),
                });

            await this.dbContext.Set<Transaction>()
                .AddAsync(new Transaction
                {
                    Amount = 30,
                    TransactionCategoryId = cat2.Entity.TransactionCategoryId,
                    UserId = this.userPackage.UserId,
                    TransactionDate = DateTime.UtcNow.AddMonths(-1),
                });
            
            await this.dbContext.SaveChangesAsync();
        }
    }
}
