using MoneySaver.Api.Data;
using MoneySaver.Api.Models;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Models.Response;
using MoneySaver.System.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IBudgetService
    {
        Task<BudgetResponseModel> CreateBudget(CreateBudgetRequest model);
        Task<BudgetModel> GetBudgetItems();

        Task<IEnumerable<BudgetItemModel>> GetBudgetItemsAsync(int budgetId);

        Task<BudgetResponseModel> GetCurrentInUseAsync();

        Task<BudgetItemModel> AddItemAsync(int budgetId, BudgetItemModel budgetItemModel);

        Task<BudgetItemModel> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel);
        Task RemoveItemAsync(int budgetId,int itemId);

        Task<Result<Budget>> CopyBudgetAsync(int budgetId, bool setToBeInUse = false);
    }
}
