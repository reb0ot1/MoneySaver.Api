using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
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
        private IRepository<Budget> budgetRepository;

        public BudgetController(ILogger<BudgetController> logger, IRepository<Budget> budgetRepository)
        {
            this.logger = logger;
            this.budgetRepository = budgetRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = this.budgetRepository.GetAll().ToList();

            return this.Ok(result
               .Select(bd =>
                   new BudgetModel
                   {
                       Id = bd.Id,
                       Type = bd.Type
                   })
               );
        }
    }
}
