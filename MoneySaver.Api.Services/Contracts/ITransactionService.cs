﻿using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using System;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionService
    {
        Task<PageModel<TransactionModel>> GetTransactionsForPageAsync(PageRequest pageRequest);
        Task<TransactionModel> GetTransactionAsync(Guid id);
        Task<TransactionModel> UpdateTransactionAsync(TransactionModel transaction);
        Task<TransactionModel> CreateTransactionAsync(TransactionModel transactionModel);
        Task RemoveTransactionAsync(Guid id);
    }
}
