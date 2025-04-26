using AutoMapper.Configuration.Annotations;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Services.Implementation;
using MoneySaver.API.Test.SeedData;

namespace MoneySaver.API.Test.IntegrationTests
{
    // [Collection(nameof(BudgetCollection))]
    public class TransactionServiceTest : IClassFixture<TransactionContext>
    {
        private readonly TransactionContext _transactionContext;

        public TransactionServiceTest(TransactionContext context)
        {
           this._transactionContext = context;
           _transactionContext.ClearDataAsync().GetAwaiter().GetResult();
           _transactionContext.SeedDataAsync().GetAwaiter().GetResult();
        }

        //TODO: Create transaction
        //TODO: Update transaction
        //TODO: Delete transaction
        //TODO: Search transaction

        [Fact]
        public async void TransactionService_CreateTransaction_ReturnTransaction()
        {
            TransactionService service = this._transactionContext.TransactionService;
            TransactionCategoryService transactionCategoryService = this._transactionContext.TransactionCategoryService;
            var pageModel = await service.GetTransactionsForPageAsync(new PageRequest { ItemsPerPage = 1, ItemsToSkip = 0 });
            var totalCount = pageModel.TotalCount;
            var transactionToUse = pageModel.Result.FirstOrDefault();
            var categoryId = transactionToUse.TransactionCategoryId;
            var dateTimeNow = this._transactionContext.DateProvider.GetDateTimeNow();
            var newTransactionModel = new TransactionModel
            {
                Amount = 150,
                TransactionCategoryId = categoryId,
                TransactionDate = dateTimeNow.AddMonths(-1)
            };
            
            var transactionCreationgRequest = await service.CreateTransactionAsync(newTransactionModel);

            var getTransaction = await service.GetTransactionAsync(Guid.Parse(transactionCreationgRequest.Id));
            
            Assert.NotNull(getTransaction);
            Assert.Equal(newTransactionModel.Amount, getTransaction.Amount);
            Assert.Equal(newTransactionModel.TransactionCategoryId, getTransaction.TransactionCategoryId);
            Assert.Equal(newTransactionModel.TransactionDate, getTransaction.TransactionDate);
        }

        // [Fact]
        // public async void BudgetService_GetBudgetItems_ReturnBudgetItems()
        // {
        //     //Arrange
        //     var service = this.budgetContext.GetBudgetService;
        //     var categoryService = this.budgetContext.GetTransactionCategoryService;
        //     var budgetItemsService = this.budgetContext.GetBudgetItemsService;
        //     var transactionService = this.budgetContext.GetTransactionService;
        //     var budgetPage = await service.GetBudgetsPerPageAsync(1, 1);
        //     var budget = budgetPage.Data.Result.FirstOrDefault();
        //     var budgetToUseId = budget.Id;
        //     var items = await budgetItemsService.GetItemsAsync(budget.Id);
        //     var initialCount = items.Count();
        //     
        //     var categoryToAddFirst =  new TransactionCategoryModel
        //     {
        //         Name = "Test category",
        //     };
        //     
        //     var categoryToAddSecond =  new TransactionCategoryModel
        //     {
        //         Name = "Test category2",
        //     };
        //
        //     var categoryFirst = await categoryService.CreateCategoryAsync(categoryToAddFirst);
        //     var categorySecond = await categoryService.CreateCategoryAsync(categoryToAddSecond);
        //     int category1Id = (int)categoryFirst.Data.TransactionCategoryId;
        //     int category2Id = (int)categorySecond.Data.TransactionCategoryId;
        //     var itemToAddFirst =  new BudgetItemModel
        //     {
        //         LimitAmount = 200,
        //         TransactionCategoryId = category1Id 
        //     };
        //     
        //     var itemToAddSecond =  new BudgetItemModel
        //     {
        //         LimitAmount = 300,
        //         TransactionCategoryId = category2Id
        //     };
        //     
        //     var budgetItemFirstAdded = await service
        //         .AddItemAsync(budget.Id, itemToAddFirst);
        //     
        //     var budgetItemSecondAdded = await service
        //         .AddItemAsync(budget.Id, itemToAddSecond);
        //
        //     var transactionToAddFirst = new TransactionModel
        //     {
        //         Amount = 10,
        //         TransactionCategoryId = category1Id,
        //         TransactionDate = DateTime.UtcNow.AddMonths(-1),
        //     };
        //     
        //     var transactionToAddSecond = new TransactionModel
        //     {
        //         Amount = 30,
        //         TransactionCategoryId = category2Id,
        //         TransactionDate = DateTime.UtcNow.AddMonths(-1),
        //     };
        //
        //     await transactionService.CreateTransactionAsync(transactionToAddFirst);
        //     await transactionService.CreateTransactionAsync(transactionToAddSecond);
        //     
        //     //Act
        //     var testResult = await this.budgetContext
        //         .GetBudgetService
        //         .GetSpentAmountsAsync(budgetToUseId);
        //
        //     //Assert
        //     Assert.NotNull(testResult);
        //     Assert.NotNull(testResult.Data);
        //     Assert.NotNull(testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category1Id));
        //     Assert.Equal(transactionToAddFirst.Amount, testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category1Id).SpentAmount);
        //     Assert.NotNull(testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category2Id));
        //     Assert.Equal(transactionToAddSecond.Amount, testResult.Data.FirstOrDefault(e => e.TransactionCategoryId == category2Id).SpentAmount);
        //     Assert.Equal(initialCount + 2, testResult.Data.Count());
        //     Assert.True(testResult.Data.All(e => !string.IsNullOrEmpty(e.TransactionCategoryName)));
        // }
        //
        // [Fact]
        // public async void BudgetService_RemoveItem()
        // {
        //     //Arrange
        //     var service = this.budgetContext.GetBudgetService;
        //     var categoryService = this.budgetContext.GetTransactionCategoryService;
        //     var budgetItemsService = this.budgetContext.GetBudgetItemsService;
        //     var budgetPage = await service.GetBudgetsPerPageAsync(1, 1);
        //     var budget = budgetPage.Data.Result.FirstOrDefault();
        //     var budgetToUseId = budget.Id;
        //     var items = await budgetItemsService.GetItemsAsync(budget.Id);
        //     var initialCount = items.Count();
        //
        //     var categoryToAdd =  new TransactionCategoryModel
        //     {
        //         Name = "Test category",
        //     };
        //
        //     var categoryCreated = await categoryService.CreateCategoryAsync(categoryToAdd);
        //     
        //     var budgetItemToAdd = new BudgetItemModel
        //     {
        //         LimitAmount = 300,
        //         TransactionCategoryId = (int)categoryCreated.Data.TransactionCategoryId
        //     };
        //
        //     var res = await service
        //         .AddItemAsync(budget.Id, budgetItemToAdd);
        //     
        //     var itemsUpdated = await budgetItemsService.GetItemsAsync(budget.Id);
        //     var itemCountAfterAddedNewItem = itemsUpdated.Count();
        //     
        //     //Act
        //     await this.budgetContext.GetBudgetService
        //         .RemoveItemAsync(budgetToUseId, res.Data.Id);
        //
        //     var budgetUpdated = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);
        //     var budgetItemDeleted = budgetUpdated.BudgetItems.FirstOrDefault(e => e.TransactionCategoryId == budgetItemToAdd.TransactionCategoryId);
        //     
        //     //Assert
        //     Assert.NotNull(budgetItemDeleted);
        //     Assert.True(budgetItemDeleted.IsDeleted);
        //     Assert.Equal(initialCount, budgetUpdated.BudgetItems.Count(e => !e.IsDeleted));
        //
        // }
        //
        // [Fact]
        // public async void BudgetService_GetBudgetInUse_ReturnOneBudgetInUse()
        // {
        //     //Arrange
        //     var service = this.budgetContext.GetBudgetService;
        //     var budgetPage = await service.GetBudgetsPerPageAsync(1, 2);
        //     var budgetNotInUse = budgetPage.Data.Result.FirstOrDefault(e => !e.IsInUse);
        //
        //     var budgetUpdateRequest = new UpdateBudgetRequest
        //     {
        //         BudgetType = budgetNotInUse.BudgetType,
        //         StartDate = budgetNotInUse.StartDate,
        //         EndDate = budgetNotInUse.EndDate,
        //         Name = budgetNotInUse.Name,
        //         IsInUse = true
        //     };
        //     
        //     var updateBudgetRequestResult = await service.UpdateBudgetAsync(budgetNotInUse.Id, budgetUpdateRequest);
        //
        //     //Act
        //     var budgetInUseResult = await service
        //         .GetCurrentInUseAsync();
        //
        //     //Assert
        //     Assert.NotNull(budgetInUseResult);
        //     Assert.True(budgetInUseResult.Succeeded);
        //     Assert.Equal(budgetInUseResult.Data.Id, budgetNotInUse.Id);
        //     Assert.Equal(budgetInUseResult.Data.Name, budgetNotInUse.Name);
        //     Assert.Equal(budgetInUseResult.Data.StartDate, budgetNotInUse.StartDate);
        //     Assert.Equal(budgetInUseResult.Data.EndDate, budgetNotInUse.EndDate);
        // }
        //
        // [Fact]
        // public async void BudgetService_EditBudgetItem_ReturnUpdatedBudgetItem()
        // {
        //     //Arrange
        //     var service = this.budgetContext.GetBudgetService;
        //     var categoryService = this.budgetContext.GetTransactionCategoryService;
        //     var budgetPage = await service.GetBudgetsPerPageAsync(1, 2);
        //     var budgetToUse = budgetPage.Data.Result.FirstOrDefault();
        //     var budgetToUseId = budgetToUse.Id;
        //     var budgetItems = service.GetBudgetItemsAsync(budgetToUseId);
        //     var itemToUpdate = budgetItems.Result.Data.FirstOrDefault();
        //     var newLimitAmount = itemToUpdate.LimitAmount + 100;
        //     var categoryToAdd =  new TransactionCategoryModel
        //     {
        //         Name = "Test category",
        //     };
        //
        //     var categoryCreated = await categoryService.CreateCategoryAsync(categoryToAdd);
        //     
        //     var modelForUpdate = new BudgetItemRequestModel
        //     {
        //         LimitAmount = newLimitAmount,
        //         TransactionCategoryId = categoryCreated.Data.TransactionCategoryId.Value
        //     };
        //
        //     //Act
        //     var updatedBudgetItemRequest = await service
        //         .EditItemAsync(budgetToUseId, itemToUpdate.Id, modelForUpdate);
        //
        //     var budget = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);
        //     var updatedItem = budget.BudgetItems.FirstOrDefault(e =>
        //         e.TransactionCategoryId == categoryCreated.Data.TransactionCategoryId.Value);
        //     
        //     //Assert
        //     Assert.NotNull(updatedBudgetItemRequest);
        //     Assert.True(updatedBudgetItemRequest.Succeeded);
        //     Assert.Equal(updatedBudgetItemRequest.Data.LimitAmount, modelForUpdate.LimitAmount);
        //     Assert.Equal(updatedBudgetItemRequest.Data.TransactionCategoryId, modelForUpdate.TransactionCategoryId);
        //     Assert.NotNull(updatedItem);
        //     Assert.Equal(newLimitAmount, updatedItem.LimitAmount);
        //     Assert.Null(budget.BudgetItems.FirstOrDefault(x => x.TransactionCategoryId == itemToUpdate.TransactionCategoryId));
        // }

        // public void Dispose()
        // {
        // }

    }
}
