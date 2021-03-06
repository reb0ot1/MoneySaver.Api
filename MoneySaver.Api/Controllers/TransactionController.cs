using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private ILogger<TransactionController> logger;
        private ITransactionService transactionService;

        public TransactionController(
            ILogger<TransactionController> logger,
            ITransactionService transactionService)
        {
            this.logger = logger;
            this.transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            IEnumerable<TransactionModel> result = await this.transactionService.GetAllTransactionsAsync();

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

        [HttpGet("remove/{id}")]
        public async Task<IActionResult> RemoveTransaction(Guid id)
        {
            await this.transactionService.RemoveTransactionAsync(id);

            return this.Ok();
        }
    }
}
