using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Models.Reports;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly IReportsService reportsService;

        public ReportsController(IReportsService reportsService)
        {
            this.reportsService = reportsService;
        }

        [HttpPost("expenses")]
        public async Task<IActionResult> GetExpensesByCategory(FilterModel filter)
        {
            IEnumerable<DataItem> result = await this.reportsService.GetExpensesPerCategoryAsync(filter);

            return this.Ok(result);
        }

        [HttpPost("expensesperiod")]
        public async Task<IActionResult> GetExpensesInPeriod(FilterModel filter)
        {
            LineChartData result = await this.reportsService.GetExpensesByPeriod(filter);

            return this.Ok(result);
        }

        [HttpPost("expensesbycategories")]
        public async Task<IActionResult> GetExpensesForPeriodByCategoriesAsync(FilterModel filter)
        {
            LineChartData result = await this.reportsService.GetExpensesForPeriodByCategoriesAsync(filter);

            return this.Ok(result);
        }

        [HttpPost("spentAmountByCategory")]
        public async Task<IActionResult> GetTransactionAmountSpentByCategory(PageRequest pageRequest)
        {
            var result = await this.reportsService.SpentAmountPerCategorieAsync(pageRequest);
            return this.Ok(result);
        }
    }
}
