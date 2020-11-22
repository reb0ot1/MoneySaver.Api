using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
