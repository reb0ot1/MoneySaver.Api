﻿using MoneySaver.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionModel>> GetAllTransactionsAsync();
        Task<TransactionModel> GetTransactionAsync(Guid id);
        Task<TransactionModel> UpdateTransactionAsync(TransactionModel transaction);
        Task<TransactionModel> CreateTransactionAsync(TransactionModel transactionModel);
        Task RemoveTransactionAsync(Guid id);
    }
}
