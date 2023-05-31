using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IBudgetItemService
    {
        Task<BudgetItemModel> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel);

        Task<IEnumerable<BudgetItemSpentModel>> GetItemsAsync(int budgetId);

        Task RemoveItemAsync(int budgetId, int itemId);
    }
}
