﻿using MoneySaver.Api.Models;
using System.Collections.Generic;
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
        Task<BudgetItemModel> AddItemAsync(BudgetItemModel budgetItem);
        Task<BudgetItemModel> EditItemAsync(BudgetItemModel budgetItemModel);
        Task RemoveItemAsync(int id);
    }
}
