using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private ITransactionService transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        [HttpPost("page")]
        public async Task<IActionResult> GetTransactionsPerPage(PageRequest page)
        {
            PageModel<TransactionModel> result = await this.transactionService.GetTransactionsForPageAsync(page);

            return this.Ok(result);
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransaction(Guid transactionId)
        {
            TransactionModel result = await this.transactionService.GetTransactionAsync(transactionId);

            return this.Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(TransactionModel transactionModel)
        {
            TransactionModel result = await this.transactionService.UpdateTransactionAsync(transactionModel);

            return this.Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionModel transactionModel)
        {
            TransactionModel result = await this.transactionService.CreateTransactionAsync(transactionModel);
            return this.Ok(result);
        }

        [HttpPost("spentAmountByCategory")]
        public async Task<IActionResult> GetTransactionAmountSpentByCategory(PageRequest pageRequest)
        {
            var result = await this.transactionService.SpentAmountPerCategorieAsync(pageRequest.BudgetType, pageRequest.ItemsPerPage);
            return this.Ok(result);
        }

        [HttpGet("remove/{id}")]
        public async Task<IActionResult> RemoveTransaction(Guid id)
        {
            await this.transactionService.RemoveTransactionAsync(id);

            return this.Ok();
        }
    }
}
