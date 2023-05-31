using MoneySaver.Api.Data;
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
        Task<Result<BudgetResponseModel>> GetBudgetAsync(int id);

        Task<Result<PageModel<BudgetResponseModel>>> GetBudgetsPerPageAsync(int page, int pageSize);

        Task<Result<BudgetResponseModel>> CreateBudgetAsync(CreateBudgetRequest model);

        Task<Result<BudgetResponseModel>> UpdateBudgetAsync(int id, UpdateBudgetRequest model);

        Task<Result<IEnumerable<BudgetItemModel>>> GetBudgetItemsAsync(int budgetId);

        Task<Result<BudgetResponseModel>> GetCurrentInUseAsync();

        Task<Result<BudgetItemModel>> AddItemAsync(int budgetId, BudgetItemModel budgetItemModel);

        Task<Result<BudgetItemModel>> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel);

        Task<Result<bool>> RemoveItemAsync(int budgetId,int itemId);

        Task<Result<Budget>> CopyBudgetAsync(int budgetId, bool setToBeInUse = false);

        Task<Result<IEnumerable<BudgetItemSpentModel>>> GetSpentAmountsAsync(int budgetId);
    }
}
