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
            //TODO: Add check for return bad request if needed
            PageModel<TransactionModel> result = await this.transactionService.GetTransactionsForPageAsync(page);

            return this.Ok(result);
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransaction(Guid transactionId)
        {
            //TODO: Add check for return bad request if needed
            TransactionModel result = await this.transactionService.GetTransactionAsync(transactionId);

            return this.Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(TransactionModel transactionModel)
        {
            //TODO: Add check for return bad request if needed
            TransactionModel result = await this.transactionService.UpdateTransactionAsync(transactionModel);

            return this.Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionModel transactionModel)
        {
            //TODO: Add check for return bad request if needed
            TransactionModel result = await this.transactionService.CreateTransactionAsync(transactionModel);
            return this.Ok(result);
        }

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveTransaction(Guid id)
        {
            //TODO: Add check for return bad request if needed
            await this.transactionService.RemoveTransactionAsync(id);

            return this.Ok();
        }
    }
}
