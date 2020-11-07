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
    public class CategoryController : Controller
    {
        //TODO: Endpoint for getting all categories
        //TODO: Endpoint for getting category by id
        //TODO: Endpoint for Update category by id
        //TODO: Endpoint for Add category
        //TODO: Endpoint for delete category???
        private ILogger<TransactionController> logger;
        private IRepository<TransactionCategory> categoryRepository;

        public CategoryController(ILogger<TransactionController> logger, IRepository<TransactionCategory> categoryRepository)
        {
            this.logger = logger;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = this.categoryRepository
                .GetAll()
                .ToList();

            return this.Ok(result
                .Select(ct =>
                    new TransactionCategoryModel
                    {
                        TransactionCategoryId = ct.TransactionCategoryId,
                        Name = ct.Name
                    })
                );
        }
    }
}
