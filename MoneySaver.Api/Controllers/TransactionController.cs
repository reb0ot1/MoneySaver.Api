using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;

namespace MoneySaver.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private ILogger<TransactionController> logger;
        private ITransactionService transactionService;


        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            this.logger = logger;
            this.transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return this.Ok(this.transactionService.GetAllTransactions());
        }

        [HttpGet("{transactionId}")]
        public IActionResult GetTransaction(Guid transactionId)
        {
            return this.Ok(this.transactionService.GetTransaction(transactionId));
        }

        [HttpPut]
        public IActionResult UpdateTransaction(TransactionModel transactionModel)
        {
            return this.Ok(this.transactionService.UpdateTransaction(transactionModel));
        }

        [HttpPost]
        public IActionResult CreateTransaction(TransactionModel transactionModel)
        {
            return this.Ok(this.transactionService.CreateTransaction(transactionModel));
        }

        [HttpDelete]
        public void RemoveTransaction(Guid transactionId)
        {
            this.transactionService.RemoveTransaction(transactionId);
        }
    }
}
