using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : Controller
    {
        private ILogger<TransactionController> logger;
        private ITransactionCategoryService categoryService;

        public CategoryController(ILogger<TransactionController> logger, ITransactionCategoryService categoryService)
        {
            this.logger = logger;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<TransactionCategoryModel> result
                = await this.categoryService.GetAllCategoriesAsync();

            return this.Ok(result);
        }

        [HttpGet("{transactionCategoryId}")]
        public IActionResult GetCategory(int transactionCategoryId)
        {
            return this.Ok(this.categoryService.GetCategory(transactionCategoryId));
        }

        [HttpPut]
        public IActionResult UpdateCategory(TransactionCategoryModel transactionCategoryModel)
        {
            return this.Ok(this.categoryService.UpdateCategory(transactionCategoryModel));
        }

        [HttpPost]
        public IActionResult CreateCategory(TransactionCategoryModel transactionCategoryModel)
        {
            return this.Ok(this.categoryService.CreateCategory(transactionCategoryModel));
        }

        [HttpDelete]
        public void RemoveTransaction(int transactionCategoryId)
        {
            this.categoryService.RemoveCategory(transactionCategoryId);
        }
    }
}
