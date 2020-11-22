using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace MoneySaver.Api.Services.Contracts
{
    public interface ITransactionService
    {
        List<TransactionModel> GetAllTransactions();
        TransactionModel GetTransaction(Guid id);
        TransactionModel UpdateTransaction(TransactionModel transaction);
        TransactionModel CreateTransaction(TransactionModel transactionModel);
        void RemoveTransaction(Guid id);


    }
}
