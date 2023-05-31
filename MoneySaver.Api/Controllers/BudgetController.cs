using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.System.Services;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : Controller
    {
        private IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetBudgetsPerPage([FromQuery] int page, [FromQuery] int pageSize)
        {
            Result<PageModel<BudgetResponseModel>> result = await this.budgetService.GetBudgetsPerPageAsync(page, pageSize);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpGet("inuse")]
        public async Task<IActionResult> GetCurrentInUse()
        {
            var result = await this.budgetService.GetCurrentInUseAsync();

            if (result.Succeeded)
            { 
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpGet("{budgetId}/items")]
        public async Task<IActionResult> GetBudgetItemsAsync(int budgetId)
        {
            //TODO: Add check if something went wrong
            var result = await this.budgetService.GetSpentAmountsAsync(budgetId: budgetId);

            if (result.Succeeded)
            { 
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPost("{budgetId}/additem")]
        public async Task<IActionResult> AddBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            //TODO: Add check if something went wrong
            var result = await this.budgetService.AddItemAsync(budgetId, budgetItem);
            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPut("{budgetId}/updateItem/{itemId}")]
        public async Task<IActionResult> UpdateBudgetItem(int budgetId, int itemId, BudgetItemRequestModel budgetItem)
        {
            var result = await this.budgetService.EditItemAsync(budgetId, itemId, budgetItem);
            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpDelete("{budgetId}/removeitem/{itemId}")]
        public async Task<IActionResult> RemoveItem2(int budgetId, int itemId)
        {
            var result = await this.budgetService.RemoveItemAsync(budgetId, itemId);
            if (result.Succeeded)
            {
                return this.Ok();
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBudget(CreateBudgetRequest model)
        {
            var result = await this.budgetService.CreateBudgetAsync(model);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPost("{id}/copy")]
        public async Task<IActionResult> CopyBudget(int id)
        {
            var result = await this.budgetService.CopyBudgetAsync(id);
            if (result.Succeeded)
            { 
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudget(int id)
        {
            var result = await this.budgetService.GetBudgetAsync(id);
            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetRequest model)
        {
            var result = await this.budgetService.UpdateBudgetAsync(id, model);
            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }
    }
}
