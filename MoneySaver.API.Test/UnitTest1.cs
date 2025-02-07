using AutoMapper;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Implementation;
using Moq;

namespace MoneySaver.API.Test
{

    public class UnitTest1
    {

        private readonly string userId = Guid.NewGuid().ToString();

        //[Fact]
        //[Theory]
        //[InlineData]
        //[ClassData]
        //public async void Test_TestService()
        //{
        //    var budgetsList = new List<Budget> { new Budget() { IsCurenttlyInUse = true, EndDate = DateTime.UtcNow.AddDays(-1), Name = "Test1", UserId = this.userId } };
        //    var mockRep = new Mock<IRepository<Budget>>();
        //    var budgetService = new TestService(mockRep.Object);

        //    var mock = budgetsList.BuildMock();

        //    mockRep.Setup(x => x.GetAll()).Returns(mock);

        //    var res = await budgetService.GetAllBudgets();
        //    var existsInUse = res.Any(e => e.IsCurenttlyInUse);
        //    Assert.True(existsInUse);
        //}

        [Fact]
        public async void Test_GetBudgetInUse()
        {
            //Arrange
            //Act
            //Assert
            var myProfile = new MoneySaver.Api.Mapper();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);

            var budgetInUseFake = new Budget() { IsInUse = true, EndDate = DateTime.UtcNow.AddDays(-1), Name = "Test1", UserId = this.userId };
            var budgetsList = new List<Budget> { budgetInUseFake };
            var mockRepBudget = new Mock<IRepository<Budget>>();
            var mockRepBudgetItem = new Mock<IRepository<BudgetItem>>();
            var mockRepBudgetCategoryTransaction = new Mock<IRepository<TransactionCategory>>();
            var mockRepBudgetTransaction = new Mock<IRepository<Transaction>>();
            //var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<BudgetService>>();
            var userPackage = new UserPackage { IsAdmin = true, UserId = this.userId };
            var budgetService = new BudgetService(
                mockRepBudget.Object,
                mockRepBudgetItem.Object,
                mockRepBudgetCategoryTransaction.Object,
                mockRepBudgetTransaction.Object,
                mapper,
                mockLogger.Object,
                userPackage,
                new DateProvider());

            var mock = budgetsList.BuildMock();

            mockRepBudget.Setup(x => x.GetAll()).Returns(mock);
            //mockMapper.Setup(x => x.Map(It.())).Returns(new BudgetResponseModel() { EndDate = DateTime.UtcNow.AddDays(-1), Name = "Test1" });

            var res = await budgetService.GetCurrentInUseAsync();
            Assert.NotNull(res);
        }
    }
}