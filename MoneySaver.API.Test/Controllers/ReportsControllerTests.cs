using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoneySaver.Api.Controllers;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Shared;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.System.Services;

namespace MoneySaver.API.Test.Controllers
{
    public class ReportsControllerTests
    {
        private readonly Mock<IReportsService> reportsService = new();
        private readonly ReportsController controller;

        private static readonly FilterModel Filter = new()
        {
            From = new DateTime(2025, 1, 1),
            To = new DateTime(2025, 12, 31)
        };

        public ReportsControllerTests()
        {
            this.controller = new ReportsController(this.reportsService.Object);
        }

        [Fact]
        public void Controller_RequiresAuthorization()
        {
            Assert.NotNull(Attribute.GetCustomAttribute(typeof(ReportsController), typeof(AuthorizeAttribute)));
        }

        [Fact]
        public async Task GetExpensesByCategory_ReturnsOkWithData_WhenServiceSucceeds()
        {
            var data = new List<DataItem> { new() { Name = "Food", Amount = 12.5, Y = 1 } };
            this.reportsService.Setup(s => s.GetExpensesPerCategoryAsync(Filter))
                .ReturnsAsync(Result<List<DataItem>>.SuccessWith(data));

            var result = await this.controller.GetExpensesByCategory(Filter);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(data, ok.Value);
        }

        [Fact]
        public async Task GetExpensesByCategory_ReturnsBadRequestWithFirstError_WhenServiceFails()
        {
            this.reportsService.Setup(s => s.GetExpensesPerCategoryAsync(Filter))
                .ReturnsAsync(Result<List<DataItem>>.Failure(new[] { "first error", "second error" }));

            var result = await this.controller.GetExpensesByCategory(Filter);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("first error", badRequest.Value);
        }

        [Fact]
        public async Task GetExpensesInPeriod_ReturnsOkWithChartData()
        {
            var chart = new LineChartData { Categories = new[] { "1/2025" }, Series = new List<SeriesItem>() };
            this.reportsService.Setup(s => s.GetExpensesByPeriod(Filter)).ReturnsAsync(chart);

            var result = await this.controller.GetExpensesInPeriod(Filter);

            Assert.Same(chart, Assert.IsType<OkObjectResult>(result).Value);
        }

        [Fact]
        public async Task GetExpensesForPeriodByCategories_ReturnsOkWithChartData()
        {
            var chart = new LineChartData { Categories = new[] { "1/2025" }, Series = new List<SeriesItem>() };
            this.reportsService.Setup(s => s.GetExpensesForPeriodByCategoriesAsync(Filter)).ReturnsAsync(chart);

            var result = await this.controller.GetExpensesForPeriodByCategoriesAsync(Filter);

            Assert.Same(chart, Assert.IsType<OkObjectResult>(result).Value);
        }

        [Fact]
        public async Task GetTransactionAmountSpentByCategory_ReturnsOkWithItems()
        {
            var request = new PageRequest { ItemsPerPage = 10, Filter = Filter };
            IEnumerable<IdValue<double?>> items = new[] { new IdValue<double?> { Id = 1, Value = 5 } };
            this.reportsService.Setup(s => s.SpentAmountPerCategorieAsync(request)).ReturnsAsync(items);

            var result = await this.controller.GetTransactionAmountSpentByCategory(request);

            Assert.Same(items, Assert.IsType<OkObjectResult>(result).Value);
        }
    }
}
