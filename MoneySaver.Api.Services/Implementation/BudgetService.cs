using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Services.Contracts;
using MoneySaver.Api.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneySaver.Api.Services.Implementation
{
    public class BudgetService : IBudgetService
    {
        private IRepository<Budget> budgetRepository;
        private IRepository<BudgetItem> budgetItemRepository;
        private IRepository<Transaction> transactionRepository;
        private IMapper mapper;
        private IRepository<TransactionCategory> transactionCategory;

        public BudgetService(
            IRepository<Budget> budgetRepository, 
            IRepository<BudgetItem> budgetItemRepository, 
            IRepository<TransactionCategory> transactionCategory,
            IRepository<Transaction> transactionRepository,
            IMapper mapper)
        {
            this.budgetRepository = budgetRepository;
            this.budgetItemRepository = budgetItemRepository;
            this.mapper = mapper;
            this.transactionCategory = transactionCategory;
            this.transactionRepository = transactionRepository;
        }

        public BudgetModel CreateBudget(BudgetModel budgetModel)
        {
            Budget budget = mapper.Map<Budget>(budgetModel);
            this.budgetRepository.AddAsync(budget);

            return budgetModel;
        }

        public List<BudgetModel> GetAllBudgets()
        {
            List<BudgetModel> budgetModels = budgetRepository.GetAll().Select(m => mapper.Map<BudgetModel>(m)).ToList();

            return budgetModels;
        }

        public BudgetModel GetBudget(int id)
        {
            Budget budget = this.budgetRepository
                .GetAll()
                .FirstOrDefault(b => b.Id == id);

            BudgetModel budgetModel = mapper.Map<BudgetModel>(budget);

            return budgetModel;
        }

        public async Task<BudgetModel> GetBudgetItems(Services.Models.Enums.BudgetType budgetType)
        {
            //TODO: Add filtration by user id
            var query = from budgetItem in this.budgetItemRepository.GetAll()
                        join trans in this.transactionCategory.GetAll()
                            on budgetItem.TransactionCategoryId equals trans.TransactionCategoryId
                        select new { budgetItem, trans };

            var result = await query.ToListAsync();
            var transactionCategoryIds = result.Select(s => s.trans.TransactionCategoryId);
            //var categoryIds = budgetItems.Select(s => s.TransactionCategoryId);
            var now = DateTime.UtcNow;
            var firstDayOfTheMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfTheMonth = firstDayOfTheMonth.AddMonths(1).AddDays(-1);

            var transactions = await this.transactionRepository
                .GetAll()
                .Where(w => transactionCategoryIds.Contains(w.TransactionCategoryId) && w.TransactionDate >= firstDayOfTheMonth && w.TransactionDate <= lastDayOfTheMonth)
                .ToListAsync();

            List<BudgetItemModel> budgetItems = new List<BudgetItemModel>();
            foreach (var item in result)
            {
                var budgetItemModel = new BudgetItemModel
                {
                    Id = item.budgetItem.BudgetItemId,
                    TransactionCategoryId = item.trans.TransactionCategoryId,
                    LimitAmount = item.budgetItem.LimitAmount,
                    SpentAmount = transactions.
                        Where(w => w.TransactionCategoryId == item.budgetItem.TransactionCategoryId)
                        .Sum(s => s.Amount)
                };

                budgetItemModel.CalculateProgress();

                budgetItems.Add(budgetItemModel);
            }

            //IEnumerable<TransactionCategory> categories = this.transactionCategory
            //    .GetAll()
            //    .Where(e => categoryIds.Contains(e.TransactionCategoryId));
            var budgetModel = new BudgetModel
            {
                BudgetItems = budgetItems,
                LimitAmount = budgetItems.Sum(s => s.LimitAmount),
                TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount),
            };

            budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            return budgetModel;
        }

        public void RemoveBudget(int id)
        {
            Budget budget = this.budgetRepository.GetAll().FirstOrDefault(b => b.Id == id);
            budget.IsDeleted = true;
            budget.DeletedOnUtc = DateTime.UtcNow;
            this.budgetRepository.RemoveAsync(budget);
        }

        public BudgetModel UpdateBudget(BudgetModel budgetModel)
        {
            Budget budget = mapper.Map<Budget>(budgetModel);
            this.budgetRepository.UpdateAsync(budget);

            return budgetModel;
        }
    }
}
