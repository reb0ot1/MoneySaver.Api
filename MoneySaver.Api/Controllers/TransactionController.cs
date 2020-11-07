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
    public class TransactionController : Controller
    {
        private ILogger<TransactionController> logger;
        private IRepository<Transaction> transactionRepository;

        public TransactionController(ILogger<TransactionController> logger, IRepository<Transaction> repository)
        {
            this.logger = logger;
            this.transactionRepository = repository;
            //TODO: Add endpoint for getting all transactions
            //TODO: Add endpoint for getting transaction by id
            //TODO: Add endpoint for updating transation by id;
            //TODO: Add endpoing for adding transaction
            //TODO: Add endpoint for deleting transaction

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = this.transactionRepository.GetAll().ToList();

            return this.Ok(result
                .Select(trm => 
                    new TransactionModel 
                    {
                        Id = trm.Id.ToString(),
                        TransactionCategoryId = trm.TransactionCategoryId,
                        AdditionalNote = trm.AdditionalNote, 
                        Amount = trm.Amount,
                        TransactionDate = trm.TransactionDate
                    })
                );
        }
    }
}
