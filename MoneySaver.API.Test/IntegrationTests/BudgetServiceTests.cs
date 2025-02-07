using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.API.Test.SeedData;

namespace MoneySaver.API.Test.IntegrationTests
{
    [Collection(nameof(BudgetCollection))]
    public class BudgetServiceTests //: IClassFixture<BudgetContext>
    {
        private readonly BudgetContext budgetContext;

        public BudgetServiceTests(BudgetContext context)
        {
           this.budgetContext = context;
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
            var budgetIdToUse = 2;

            var budgetItemToAdd = new BudgetItemModel
            {
                LimitAmount = 300,
                TransactionCategoryId = 3
            };

            var service = this.budgetContext.GetBudgetService;

            //Act
            var res = await service
                .AddItemAsync(budgetIdToUse, budgetItemToAdd);

            //Assert
            Assert.NotNull(res);
            Assert.True(res.Succeeded);
            Assert.NotEqual(res.Data.Id, 0);
            Assert.Equal(res.Data.LimitAmount, budgetItemToAdd.LimitAmount);
            Assert.Equal(res.Data.TransactionCategoryId, budgetItemToAdd.TransactionCategoryId);
        }

        [Fact]
        public async void BudgetService_CopyBydget_ReuturnBudget()
        {
            //Arrange
            var budgetToCopyId = 1;
            var nowDate = this.budgetContext.GetDateProvider().GetDateTimeNow();

            //Act
            var testResult = await this.budgetContext
                .GetBudgetService
                .CopyBudgetAsync(budgetToCopyId, true);

            //Assert
            Assert.NotNull(testResult);
            Assert.NotNull(testResult.Data);
            Assert.True(testResult.Data.IsInUse);
            Assert.Equal(testResult.Data.StartDate, new DateTime(nowDate.Year, nowDate.Month, 1));
            Assert.Equal(testResult.Data.EndDate, new DateTime(nowDate.Year, nowDate.Month, 1).AddMonths(1).AddTicks(-1));
        }

        [Fact]
        public async void BudgetService_GetBudgetItems_ReturnBudgetItems()
        {
            //Arrange
            var budgetToUseId = 1;
            var category1Id = 1;
            var category2Id = 2;

            //Act
            var testResult = await this.budgetContext
                .GetBudgetService
                .GetSpentAmountsAsync(budgetToUseId);

            //Assert
            Assert.NotNull(testResult);
            Assert.NotNull(testResult.Data);
            Assert.NotNull(testResult.Data.FirstOrDefault(e => e.Id == category1Id));
            Assert.Equal(10, testResult.Data.FirstOrDefault(e => e.Id == category1Id).SpentAmount);
            Assert.NotNull(testResult.Data.FirstOrDefault(e => e.Id == category2Id));
            Assert.Equal(30, testResult.Data.FirstOrDefault(e => e.Id == category2Id).SpentAmount);
            Assert.Equal(3, testResult.Data.Count());
            Assert.True(testResult.Data.All(e => !string.IsNullOrEmpty(e.TransactionCategoryName)));
        }

        [Fact]
        public async void BudgetService_RemoveItem()
        {
            //Arrange
            var budgetToUseId = 1;
            var categoryItemToRepomoId = 3;
            var itemsCount = 2;

            //Act
            await this.budgetContext.GetBudgetService
                .RemoveItemAsync(budgetToUseId, categoryItemToRepomoId);

            var budgetUpdated = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);
            var budgetItemDeleted = budgetUpdated.BudgetItems.FirstOrDefault(e => e.TransactionCategoryId == categoryItemToRepomoId);
            
            //Assert
            Assert.NotNull(budgetItemDeleted);
            Assert.True(budgetItemDeleted.IsDeleted);
            Assert.Equal(budgetUpdated.BudgetItems.Count(e => !e.IsDeleted), itemsCount);
        }

        [Fact]
        public async void BudgetService_GetBudgetInUse_ReturnOneBudgetInUse()
        {
            //Arrange
            var budgetToUseId = 3;
            var now = this.budgetContext.GetDateProvider().GetDateTimeNow();
            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = startMonth.AddMonths(1).AddTicks(-1);
            var budgetName = startMonth.ToString("MM") + "-" + endMonth.ToString("yyy");

            //Act
            var budgetInUse = await this.budgetContext.GetBudgetService
                .GetCurrentInUseAsync();

            //Assert
            Assert.NotNull(budgetInUse);
            Assert.True(budgetInUse.Succeeded);
            Assert.Equal(budgetInUse.Data.Id, budgetToUseId);
            Assert.Equal(budgetInUse.Data.Name, budgetName);
            Assert.Equal(budgetInUse.Data.StartDate, startMonth);
            Assert.Equal(budgetInUse.Data.EndDate, endMonth);
        }

        [Fact]
        public async void BudgetService_EditBudgetItem_ReturnUpdatedBudgetItem()
        {
            //Arrange
            var budgetToUseId = 2;
            var ItemToUpdateId = 5;
            var categoryToUpdateId = 3;
            var categoryToBeUpdatedId = 2;

            var modelForUpdate = new BudgetItemRequestModel
            {
                LimitAmount = 500,
                TransactionCategoryId = categoryToUpdateId
            };

            //Act
            var budgetInUse = await this.budgetContext.GetBudgetService
                .EditItemAsync(budgetToUseId, ItemToUpdateId, modelForUpdate);

            var budget = await this.budgetContext.GetBudgetEntityAsync(budgetToUseId);

            //Assert
            Assert.NotNull(budgetInUse);
            Assert.True(budgetInUse.Succeeded);
            Assert.Equal(budgetInUse.Data.LimitAmount, modelForUpdate.LimitAmount);
            Assert.Equal(budgetInUse.Data.TransactionCategoryId, modelForUpdate.TransactionCategoryId);
            Assert.Null(budget.BudgetItems.FirstOrDefault(x => x.TransactionCategoryId == categoryToBeUpdatedId));
            Assert.NotNull(budget.BudgetItems.FirstOrDefault(x => x.TransactionCategoryId == categoryToUpdateId));
        }
    }
}
