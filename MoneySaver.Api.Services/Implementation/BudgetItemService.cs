using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models.Budget;
using MoneySaver.Api.Models.Request;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class BudgetItemService : IBudgetItemService
    {
        private readonly ILogger<BudgetItemService> _logger;
        private readonly IRepository<BudgetItem> _budgetItemRepository;
        private readonly IRepository<TransactionCategory> _transactionCategory;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Budget> _budgetRepository;

        public BudgetItemService(
            ILogger<BudgetItemService> logger,
            IRepository<BudgetItem> budgetItemRepository,
            IRepository<TransactionCategory> transactionCategory,
            IRepository<Transaction> transactionRepository,
            IRepository<Budget> budgetRepository
            )
        {
            this._logger = logger;
            this._budgetRepository = budgetRepository;
            this._transactionRepository = transactionRepository;
            this._budgetItemRepository = budgetItemRepository;  
            this._budgetRepository = budgetRepository;
            this._transactionCategory = transactionCategory;
        }


        public async Task<BudgetItemModel> EditItemAsync(int budgetId, int budgetItemId, BudgetItemRequestModel budgetItemModel)
        {
            //TODO: validate the request model values
            try
            {
                var budgetItemEntity = await this._budgetItemRepository
                    .GetAll()
                    .FirstOrDefaultAsync(i => i.BudgetId == budgetId && i.Id == budgetItemId);

                if (budgetItemEntity == null)
                {
                    return null;
                }

                budgetItemEntity.LimitAmount = budgetItemModel.LimitAmount;
                budgetItemEntity.TransactionCategoryId = budgetItemModel.TransactionCategoryId;

                var result = await this._budgetItemRepository.UpdateAsync(budgetItemEntity);

                return new BudgetItemModel
                {
                    Id = budgetItemId,
                    LimitAmount = budgetItemModel.LimitAmount,
                    TransactionCategoryId = budgetItemModel.TransactionCategoryId,
                };
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to edit budget item with id {0}. UserId {1}", budgetItemId, budgetItemModel);
            }

            return null;
        }

        public async Task RemoveItemAsync(int budgetId, int itemId)
        {
            BudgetItem item = await this._budgetItemRepository
                .GetAll()
                .FirstOrDefaultAsync(i => i.Id == itemId && i.BudgetId == budgetId);

            item.IsDeleted = true;

            await this._budgetItemRepository.SetAsDeletedAsync(item);
        }

        public async Task<IEnumerable<BudgetItemSpentModel>> GetItemsAsync(int budgetId)
        {
            var budgetDb = await this._budgetRepository
                .GetAll()
                .FirstOrDefaultAsync(e => e.Id == budgetId);

            if (budgetDb == null)
            {
                throw new Exception("budget not found.");
            }

            //TODO: Think a better way to get the budget items and their  transactions
            var query = from budgetItem in this._budgetItemRepository.GetAll().Where(e => e.BudgetId == budgetId)
                        join trans in this._transactionCategory.GetAll().Where(e => !e.IsDeleted)
                            on budgetItem.TransactionCategoryId equals trans.TransactionCategoryId
                        select new { budgetItem, trans };

            var result = await query.ToListAsync();
            var transactionCategoryIds = result.Select(s => s.trans.TransactionCategoryId);

            var transactions = await this._transactionRepository
                .GetAll()
                .Where(w => transactionCategoryIds.Contains(w.TransactionCategoryId) &&
                            w.TransactionDate >= budgetDb.StartDate &&
                            w.TransactionDate <= budgetDb.EndDate)
                .Select(s => new { TransactionCategoryId = s.TransactionCategoryId, Amount = s.Amount })
                .ToListAsync();

            List<BudgetItemSpentModel> budgetItems = new List<BudgetItemSpentModel>();
            foreach (var item in result)
            {
                var spentAmmount = transactions
                        .Where(w => w.TransactionCategoryId == item.budgetItem.TransactionCategoryId)
                        .Select(s => s.Amount)
                        .Sum(m => m);

                var budgetItemModel = new BudgetItemSpentModel
                {
                    Id = item.budgetItem.Id,
                    TransactionCategoryId = item.trans.TransactionCategoryId,
                    TransactionCategoryName = item.trans.Name,
                    LimitAmount = item.budgetItem.LimitAmount,
                    SpentAmount = spentAmmount
                };

                budgetItemModel.CalculateProgress();

                budgetItems.Add(budgetItemModel);
            };

            return budgetItems;
        }
    }
}
