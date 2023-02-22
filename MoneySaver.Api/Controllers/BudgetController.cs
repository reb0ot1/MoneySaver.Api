using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Services.Contracts;
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

        [HttpGet("inuse")]
        public async Task<IActionResult> GetCurrentInUse()
        {
            //TODO: Add check if something went wrong
            var result = await this.budgetService.GetCurrentInUseAsync();

            return this.Ok(result);
        }

        [HttpGet("{budgetId}/items")]
        public async Task<IActionResult> GetBudgetItemsAsync(int budgetId)
        {
            //TODO: Add check if something went wrong
            var result = await this.budgetService.GetBudgetItemsAsync(budgetId);

            return this.Ok(result);
        }

        [HttpPost("{budgetId}/additem")]
        public async Task<IActionResult> AddBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            //TODO: Add check if something went wrong
            BudgetItemModel result = await this.budgetService.AddItemAsync(budgetId, budgetItem);
            return this.Ok(result);
        }

        [HttpPut("{budgetId}/updateItem/{itemId}")]
        public async Task<IActionResult> UpdateBudgetItem(int budgetId, int itemId, BudgetItemRequestModel budgetItem)
        {
            BudgetItemModel result = await this.budgetService.EditItemAsync(budgetId, itemId, budgetItem);
            if (result == null)
            {
                return this.BadRequest();
            }

            return this.Ok(result);
        }

        [HttpDelete("{budgetId}/removeitem/{itemId}")]
        public async Task<IActionResult> RemoveItem2(int budgetId, int id)
        {
            //TODO: Add check if something went wrong
            await this.budgetService.RemoveItemAsync(budgetId, id);
            return this.Ok();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBudget(CreateBudgetRequest model)
        {
            var result = await this.budgetService.CreateBudget(model);

            if (result == null)
            {
                return this.BadRequest("Something went wrong.");
            }

            return this.Ok(result);
        }

        [HttpPost("{id}/copy")]
        public async Task<IActionResult> CopyBudget(int id)
        {
            //TODO: Add check if something went wrong
            await this.budgetService.CopyBudgetAsync(id);

            return this.Ok();
        }
    }
}
