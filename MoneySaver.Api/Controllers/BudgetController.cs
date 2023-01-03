using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : Controller
    {
        private IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            this.budgetService = budgetService;
        }
        
        [HttpGet("items")]
        public async Task<IActionResult> GetBudgetItemsAsync()
        {
            var result = await this.budgetService.GetBudgetItems(BudgetType.Monthly);

            return this.Ok(result);
        }

        [HttpPost("additem")]
        public async Task<IActionResult> AddBudgetItem(BudgetItemModel budgetItem)
        {
            BudgetItemModel result = await this.budgetService.AddItemAsync(budgetItem);
            return this.Ok(result);
        }

        [HttpPut("updateitem")]
        public async Task<IActionResult> UpdateBudgetItem(BudgetItemModel budgetItem)
        {
            BudgetItemModel result = await this.budgetService.EditItemAsync(budgetItem);
            if (result == null)
            {
                return this.BadRequest();
            }

            return this.Ok(result);
        }

        [HttpDelete("removeitem/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            await this.budgetService.RemoveItemAsync(id);
            return this.Ok();
        }
    }
}
