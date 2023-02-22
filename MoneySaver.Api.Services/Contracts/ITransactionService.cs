using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.Api.Models.Shared;
using System;
using System.Collections.Generic;
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

        //TODO: Think to situate this on new place
        Task<IEnumerable<IdValue<double?>>> SpentAmountPerCategorieAsync(BudgetType budgetType, int? itemsToTake = null);
    }
}
