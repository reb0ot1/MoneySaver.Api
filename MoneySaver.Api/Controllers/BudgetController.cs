﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : Controller
    {
        private ILogger<BudgetController> logger;
        private IBudgetService budgetService;

        public BudgetController(ILogger<BudgetController> logger, IBudgetService budgetService)
        {
            this.logger = logger;
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

        [HttpGet]
        public IActionResult GetAll()
        {
            return this.Ok(this.budgetService.GetAllBudgets());
        }

        [HttpGet("{budgetId}")]
        public IActionResult GetBudget(int budgetId)
        {
            return this.Ok(this.budgetService.GetBudget(budgetId));
        }

        [HttpPut]
        public IActionResult UpdateBudget(BudgetModel budgetModel)
        {
            return this.Ok(this.budgetService.UpdateBudget(budgetModel));
        }

        [HttpPost]
        public IActionResult CreateBudget(BudgetModel budgetModel)
        {
            return this.Ok(this.budgetService.CreateBudget(budgetModel));
        }

        [HttpDelete]
        public void RemoveBudget(int budgetId)
        {
            this.budgetService.RemoveBudget(budgetId);
        }
    }
}
