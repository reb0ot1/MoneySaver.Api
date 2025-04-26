using AutoMapper.Configuration.Annotations;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.API.Test.SeedData;

namespace MoneySaver.API.Test.IntegrationTests
{
    [Collection(nameof(BudgetCollection))]
    public class BudgetServiceTests : IClassFixture<BudgetContext>
    {
        private readonly BudgetContext budgetContext;

        public BudgetServiceTests(BudgetContext context)
        {
           this.budgetContext = context;
           budgetContext.ClearDataAsync().GetAwaiter().GetResult();
           budgetContext.SeedData().GetAwaiter().GetResult();
        }
        
        [Fact]
        public async void BudgetService_CreateBudget_ReturnBudget()
        {
            //Arrange
            var dateTime = budgetContext
                .GetDateProvider()
                .GetDateTimeNow()
                .AddMonths(1);
            var monthStart = new DateTime(dateTime.Year, dateTime.Month, 1);
            var budgetToAdd = new CreateBudgetRequest
            {
                BudgetType = Api.Models.BudgetType.Monthly,
                Name = string.Format("Budget-Name-{0}", Guid.NewGuid().ToString()),
                StartDate = monthStart,
                EndDate = monthStart.AddMonths(1).AddTicks(-1) 
            };

            var service = this.budgetContext.GetBudgetService;

            //Act
            var res = await service
                .CreateBudgetAsync(budgetToAdd);
            
            //Assert
            Assert.NotNull(res);
            Assert.True(res.Succeeded);
            Assert.Equal(res.Data.Name, budgetToAdd.Name);
            Assert.Equal(res.Data.StartDate, budgetToAdd.StartDate);
            Assert.Equal(res.Data.EndDate, budgetToAdd.StartDate.AddMonths(1).AddTicks(-1));
            Assert.Equal(res.Data.BudgetType, budgetToAdd.BudgetType);
        }

        [Fact]
        public async void BudgetService_CreateBudgetWithWrongParams_ReturnNotSucceeded()
        {
            //Arrange
            var dateTime = budgetContext.GetDateProvider().GetDateTimeNow().AddMonths(1);

            var budgetToAdd = new CreateBudgetRequest
            {
                BudgetType = Api.Models.BudgetType.Monthly,
                Name = "Test Budget",
                StartDate = new DateTime(dateTime.Year, dateTime.Month, 3)
            };
            
            var service = this.budgetContext.GetBudgetService;

            //Act
            var res = await service
                .CreateBudgetAsync(budgetToAdd);

            //Assert
            Assert.NotNull(res);
            Assert.False(res.Succeeded);
        }

        [Fact]
        public async void BudgetService_AddBudgetItem_ReturnBudgetItem()
        {
            //Arrange
            var service = this.budgetContext.GetBudgetService;
            var categoryService = this.budgetContext.GetTransactionCategoryService;
            var budgetItemsService = this.budgetContext.GetBudgetItemsService;
            var budgetPage = await service.GetBudgetsPerPageAsync(1, 1);
            var budget = budgetPage.Data.Result.FirstOrDefault();
            var items = await budgetItemsService.GetItemsAsync(budget.Id);
            var initialCount = items.Count();
            
            var categoryToAdd =  new TransactionCategoryModel
            {
                Name = "Test category",
            };

            var categoryCreated = await categoryService.CreateCategoryAsync(categoryToAdd);
            
            var budgetItemToAdd = new BudgetItemModel
            {
                LimitAmount = 300,
                TransactionCategoryId = (int)categoryCreated.Data.TransactionCategoryId
            };

            //Act
            var res = await service
                .AddItemAsync(budget.Id, budgetItemToAdd);
            
            var itemsUpdated = await budgetItemsService.GetItemsAsync(budget.Id);

            //Assert
            Assert.NotNull(res);
            Assert.True(res.Succeeded);
            Assert.NotEqual(res.Data.Id, 0);
            Assert.Equal(res.Data.LimitAmount, budgetItemToAdd.LimitAmount);
            Assert.Equal(res.Data.TransactionCategoryId, budgetItemToAdd.TransactionCategoryId);
            Assert.Equal(initialCount + 1, itemsUpdated.Count());
        }

        // [Fact(Skip="Will need to be changed")]
        [Fact]
        public async void BudgetService_CopyBydget_ReturnBudget()
        {
            //Arrange
            var nowDate = this.budgetContext.GetDateProvider().GetDateTimeNow();
            var startDate = new DateTime(nowDate.Year, nowDate.Month, 1);
            var endDate = new DateTime(nowDate.Year, nowDate.Month, 1).AddMonths(1).AddTicks(-1);
            
            var service = this.budgetContext.GetBudgetService;
            var budgetItemsService = this.budgetContext.GetBudgetItemsService;
            var budgetFromPage = await service.GetBudgetsPerPageAsync(1, 1);
            var budgetToUseFromPage = budgetFromPage.Data.Result.FirstOrDefault();
            var initialBudgetItems = await budgetItemsService.GetItemsAsync(budgetToUseFromPage.Id);

            //Act
            var copiedBudget = await this.budgetContext
                .GetBudgetService
                .CopyBudgetAsync(budgetToUseFromPage.Id, true);
            
            //Assert
            Assert.NotNull(copiedBudget);
            Assert.NotNull(copiedBudget.Data);
            Assert.True(copiedBudget.Data.IsInUse);
            Assert.Equal(startDate,copiedBudget.Data.StartDate);
            Assert.Equal(endDate, copiedBudget.Data.EndDate);
            Assert.Equal(initialBudgetItems.Count(), copiedBudget.Data.BudgetItems.Count());
        }

        [Fact]
        public async void BudgetService_GetBudgetItems_ReturnBudgetItems()
        {
            //Arrange
            var service = this.budgetContext.GetBudgetService;
            var categoryService = this.budgetContext.GetTransactionCategoryService;
            var budgetItemsService = this.budgetContext.GetBudgetItemsService;
            var transactionService = this.budgetContext.GetTransactionService;
            var budgetPage = await service.GetBudgetsPerPageAsync(1, 1);
            var budget = budgetPage.Data.Result.FirstOrDefault();
            var budgetToUseId = budget.Id;
            var items = await budgetItemsService.GetItemsAsync(budget.Id);
            var initialCount = items.Count();
            
            var categoryToAddFirst =  new TransactionCategoryModel
            {
                Name = "Test category",
            };
            
            var categoryToAddSecond =  new TransactionCategoryModel
            {
                Name = "Test category2",
            };

            var categoryFirst = await categoryService.CreateCategoryAsync(categoryToAddFirst);
            var categorySecond = await categoryService.CreateCategoryAsync(categoryToAddSecond);
            int category1Id = (int)categoryFirst.Data.TransactionCategoryId;
            int category2Id = (int)categorySecond.Data.TransactionCategoryId;
            var itemToAddFirst =  new BudgetItemModel
            {
                LimitAmount = 200,
                TransactionCategoryId = category1Id 
            };
            
            var itemToAddSecond =  new BudgetItemModel
            {
                LimitAmount = 300,
                TransactionCategoryId = category2Id
            };
            
            var budgetItemFirstAdded = await service
                .AddItemAsync(budget.Id, itemToAddFirst);
            
            var budgetItemSecondAdded = await service
                .AddItemAsync(budget.Id, itemToAddSecond);

            var transactionToAddFirst = new TransactionModel
            {
                Amount = 10,
                TransactionCategoryId = category1Id,
                TransactionDate = DateTime.UtcNow.AddMonths(-1),
            };
            
            var transactionToAddSecond = new TransactionModel
            {
                Amount = 30,
                TransactionCategoryId = category2Id,
                TransactionDate = DateTime.UtcNow.AddMonths(-1),
            };

            await transactionService.CreateTransactionAsync(transactionToAddFirst);
            await transactionService.CreateTransactionAsync(transactionToAddSecond);
            
            //Act
            var testResult = await this.budgetContext
                .GetBudgetService
                .GetSpentAmountsAsync(budgetToUseId);

            //Assert
            Assert.NotNull(testResult);
            Assert.NotNull(testResult.Data);
            Assert.NotNull(testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category1Id));
            Assert.Equal(transactionToAddFirst.Amount, testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category1Id).SpentAmount);
            Assert.NotNull(testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category2Id));
            Assert.Equal(transactionToAddSecond.Amount, testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category2Id).SpentAmount);
            Assert.Equal(initialCount + 2, testResult.Data.Count());
            Assert.True(testResult.Data.All(e => !string.IsNullOrEmpty(e.TransactionCategoryName)));
        }

        [Fact]
        public async void BudgetService_RemoveItem()
        {
            //Arrange
            var service = this.budgetContext.GetBudgetService;
            var categoryService = this.budgetContext.GetTransactionCategoryService;
            var budgetItemsService = this.budgetContext.GetBudgetItemsService;
            var budgetPage = await service.GetBudgetsPerPageAsync(1, 1);
            var budget = budgetPage.Data.Result.FirstOrDefault();
            var budgetToUseId = budget.Id;
            var items = await budgetItemsService.GetItemsAsync(budget.Id);
            var initialCount = items.Count();

            var categoryToAdd =  new TransactionCategoryModel
            {
                Name = "Test category",
            };

            var categoryCreated = await categoryService.CreateCategoryAsync(categoryToAdd);
            
            var budgetItemToAdd = new BudgetItemModel
            {
                LimitAmount = 300,
                TransactionCategoryId = (int)categoryCreated.Data.TransactionCategoryId
            };

            var res = await service
                .AddItemAsync(budget.Id, budgetItemToAdd);
            
            var itemsUpdated = await budgetItemsService.GetItemsAsync(budget.Id);
            var itemCountAfterAddedNewItem = itemsUpdated.Count();
            
            //Act
            await this.budgetContext.GetBudgetService
                .RemoveItemAsync(budgetToUseId, res.Data.Id);

            var budgetUpdated = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);
            var budgetItemDeleted = budgetUpdated.BudgetItems.FirstOrDefault(e => e.TransactionCategoryId == budgetItemToAdd.TransactionCategoryId);
            
            //Assert
            Assert.NotNull(budgetItemDeleted);
            Assert.True(budgetItemDeleted.IsDeleted);
            Assert.Equal(initialCount, budgetUpdated.BudgetItems.Count(e => !e.IsDeleted));

        }

        [Fact]
        public async void BudgetService_GetBudgetInUse_ReturnOneBudgetInUse()
        {
            //Arrange
            var service = this.budgetContext.GetBudgetService;
            var budgetPage = await service.GetBudgetsPerPageAsync(1, 2);
            var budgetNotInUse = budgetPage.Data.Result.FirstOrDefault(e => !e.IsInUse);

            var budgetUpdateRequest = new UpdateBudgetRequest
            {
                BudgetType = budgetNotInUse.BudgetType,
                StartDate = budgetNotInUse.StartDate,
                EndDate = budgetNotInUse.EndDate,
                Name = budgetNotInUse.Name,
                IsInUse = true
            };
            
            var updateBudgetRequestResult = await service.UpdateBudgetAsync(budgetNotInUse.Id, budgetUpdateRequest);
    
            //Act
            var budgetInUseResult = await service
                .GetCurrentInUseAsync();

            //Assert
            Assert.NotNull(budgetInUseResult);
            Assert.True(budgetInUseResult.Succeeded);
            Assert.Equal(budgetInUseResult.Data.Id, budgetNotInUse.Id);
            Assert.Equal(budgetInUseResult.Data.Name, budgetNotInUse.Name);
            Assert.Equal(budgetInUseResult.Data.StartDate, budgetNotInUse.StartDate);
            Assert.Equal(budgetInUseResult.Data.EndDate, budgetNotInUse.EndDate);
        }

        [Fact]
        public async void BudgetService_EditBudgetItem_ReturnUpdatedBudgetItem()
        {
            //Arrange
            var service = this.budgetContext.GetBudgetService;
            var categoryService = this.budgetContext.GetTransactionCategoryService;
            var budgetPage = await service.GetBudgetsPerPageAsync(1, 2);
            var budgetToUse = budgetPage.Data.Result.FirstOrDefault();
            var budgetToUseId = budgetToUse.Id;
            var budgetItems = service.GetBudgetItemsAsync(budgetToUseId);
            var itemToUpdate = budgetItems.Result.Data.FirstOrDefault();
            var newLimitAmount = itemToUpdate.LimitAmount + 100;
            var categoryToAdd =  new TransactionCategoryModel
            {
                Name = "Test category",
            };

            var categoryCreated = await categoryService.CreateCategoryAsync(categoryToAdd);
            
            var modelForUpdate = new BudgetItemRequestModel
            {
                LimitAmount = newLimitAmount,
                TransactionCategoryId = categoryCreated.Data.TransactionCategoryId.Value
            };

            //Act
            var updatedBudgetItemRequest = await service
                .EditItemAsync(budgetToUseId, itemToUpdate.Id, modelForUpdate);

            var budget = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);
            var updatedItem = budget.BudgetItems.FirstOrDefault(e =>
                e.TransactionCategoryId == categoryCreated.Data.TransactionCategoryId.Value);
            
            //Assert
            Assert.NotNull(updatedBudgetItemRequest);
            Assert.True(updatedBudgetItemRequest.Succeeded);
            Assert.Equal(updatedBudgetItemRequest.Data.LimitAmount, modelForUpdate.LimitAmount);
            Assert.Equal(updatedBudgetItemRequest.Data.TransactionCategoryId, modelForUpdate.TransactionCategoryId);
            Assert.NotNull(updatedItem);
            Assert.Equal(newLimitAmount, updatedItem.LimitAmount);
            Assert.Null(budget.BudgetItems.FirstOrDefault(x => x.TransactionCategoryId == itemToUpdate.TransactionCategoryId));
        }

        // public void Dispose()
        // {
        // }

    }
}
