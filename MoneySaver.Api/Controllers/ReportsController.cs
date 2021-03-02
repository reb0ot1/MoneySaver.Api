using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Data;
using MoneySaver.Api.Models.Filters;
using MoneySaver.Api.Services.Contracts;
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
            var result = await this.reportsService.GetExpensesPerCategoryAsync(filter);

            return this.Ok(result);
        }
    }
}
