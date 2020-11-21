using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using MoneySaver.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace MoneySaver.Api.Controllers
{
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
            //TODO: Add endpoint for getting all transactions
            //TODO: Add endpoint for getting transaction by id
            //TODO: Add endpoint for updating transation by id;
            //TODO: Add endpoing for adding transaction
            //TODO: Add endpoint for deleting transaction

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
