using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
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
            var result = await this.categoryService.GetAllCategoriesAsync();
            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpGet("{transactionCategoryId}")]
        public async Task<IActionResult> GetCategory(int transactionCategoryId)
        {
            var result = await this.categoryService.GetCategoryAsync(transactionCategoryId);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(TransactionCategoryModel transactionCategoryModel)
        {
            var result = await this.categoryService.UpdateCategoryAsync(transactionCategoryModel);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(TransactionCategoryModel transactionCategoryModel)
        {
            var result = await this.categoryService.CreateCategoryAsync(transactionCategoryModel);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCategory(int transactionCategoryId)
        {
            var result = await this.categoryService.RemoveCategoryAsync(transactionCategoryId);

            if (result.Succeeded)
            {
                return this.Ok(result.Data);
            }

            return this.BadRequest(result.Errors);
        }
    }
}
