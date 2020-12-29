using MoneySaver.Api.Services.Models;
using MoneySaver.Api.Services.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Contracts
{
    public interface IBudgetService
    {
        List<BudgetModel> GetAllBudgets();
        BudgetModel GetBudget(int id);
        BudgetModel UpdateBudget(BudgetModel budgetModel);
        BudgetModel CreateBudget(BudgetModel budgetModel);
        void RemoveBudget(int id);

        Task<BudgetModel> GetBudgetItems(BudgetType budgetType);
    }
}
