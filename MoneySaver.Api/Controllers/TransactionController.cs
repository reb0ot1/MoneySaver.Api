using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using MoneySaver.Api.Services.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        private Authority authority;

        public TransactionController(
            ILogger<TransactionController> logger,
            ITransactionService transactionService,
            Authority authority)
        {
            this.authority = authority;
            this.logger = logger;
            this.transactionService = transactionService;
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> GetAllTransactions1()
        {
            return this.Ok();
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
            //TODO Create Middlewear for UserClaims(Id) and use it in the Service
            TransactionModel result = await this.transactionService.CreateTransactionAsync(transactionModel);

            return this.Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveTransaction(Guid transactionId)
        {
            await this.transactionService.RemoveTransactionAsync(transactionId);

            return this.Ok();
        }
    }
}
