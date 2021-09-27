using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneySaver.Api.Data;
using MoneySaver.Api.Data.Repositories;
using MoneySaver.Api.Models;
using MoneySaver.Api.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private ILogger<BudgetService> logger;
        private UserPackage userPackage;

        public BudgetService(
            IRepository<Budget> budgetRepository, 
            IRepository<BudgetItem> budgetItemRepository, 
            IRepository<TransactionCategory> transactionCategory,
            IRepository<Transaction> transactionRepository,
            IMapper mapper,
            ILogger<BudgetService> logger,
            UserPackage userPackage)
        {
            this.budgetRepository = budgetRepository;
            this.budgetItemRepository = budgetItemRepository;
            this.mapper = mapper;
            this.transactionCategory = transactionCategory;
            this.transactionRepository = transactionRepository;
            this.logger = logger;
            this.userPackage = userPackage;
        }

        public async Task<BudgetItemModel> AddItemAsync(BudgetItemModel budgetItemModel)
        {
            try
            {
                BudgetItem budgetItem = mapper.Map<BudgetItem>(budgetItemModel);
                var result = await this.budgetItemRepository.AddAsync(budgetItem);
                budgetItemModel.Id = result.Id;

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to add budget item. UserId {this.userPackage.UserId}", budgetItemModel);
            }

            return null;
        }

        public async Task<BudgetItemModel> EditItemAsync(BudgetItemModel budgetItemModel)
        {
            try
            {
                var budgetItemEntity = await this.budgetItemRepository
                .GetAll()
                .AnyAsync(i => i.Id == budgetItemModel.Id);

                if (!budgetItemEntity)
                {
                    return null;
                }

                BudgetItem budgetItem = mapper.Map<BudgetItem>(budgetItemModel);
                var result = await this.budgetItemRepository.UpdateAsync(budgetItem);

                return budgetItemModel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to edit budget item with id {budgetItemModel.Id}. UserId {this.userPackage.UserId}", budgetItemModel);
            }

            return null;
        }

        //public BudgetModel CreateBudget(BudgetModel budgetModel)
        //{
        //    Budget budget = mapper.Map<Budget>(budgetModel);
        //    this.budgetRepository.AddAsync(budget);

        //    return budgetModel;
        //}

        //public List<BudgetModel> GetAllBudgets()
        //{
        //    List<BudgetModel> budgetModels = budgetRepository.GetAll().Select(m => mapper.Map<BudgetModel>(m)).ToList();

        //    return budgetModels;
        //}

        //public BudgetModel GetBudget(int id)
        //{
        //    Budget budget = this.budgetRepository
        //        .GetAll()
        //        .FirstOrDefault(b => b.Id == id);

        //    BudgetModel budgetModel = mapper.Map<BudgetModel>(budget);

        //    return budgetModel;
        //}

        public async Task<BudgetModel> GetBudgetItems(Models.BudgetType budgetType)
        {
            //TODO: Add filtration by user id
            var query = from budgetItem in this.budgetItemRepository.GetAll().Where(e => !e.IsDeleted)
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
                .Where(w => !w.IsDeleted && 
                            transactionCategoryIds.Contains(w.TransactionCategoryId) && 
                            w.TransactionDate >= firstDayOfTheMonth && 
                            w.TransactionDate <= lastDayOfTheMonth)
                .ToListAsync();

            List<BudgetItemModel> budgetItems = new List<BudgetItemModel>();
            foreach (var item in result)
            {
                var budgetItemModel = new BudgetItemModel
                {
                    Id = item.budgetItem.Id,
                    TransactionCategoryId = item.trans.TransactionCategoryId,
                    TransactionCategoryName = item.trans.Name,
                    LimitAmount = item.budgetItem.LimitAmount,
                    SpentAmount = transactions.
                        Where(w => w.TransactionCategoryId == item.budgetItem.TransactionCategoryId)
                        .Sum(s => s.Amount)
                };

                budgetItemModel.CalculateProgress();

                budgetItems.Add(budgetItemModel);
            }

            var budgetModel = new BudgetModel
            {
                BudgetItems = budgetItems,
                LimitAmount = budgetItems.Sum(s => s.LimitAmount),
                TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount),
            };

            budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            return budgetModel;
        }

        //public void RemoveBudget(int id)
        //{
        //    Budget budget = this.budgetRepository.GetAll().FirstOrDefault(b => b.Id == id);
        //    budget.IsDeleted = true;
        //    budget.DeletedOnUtc = DateTime.UtcNow;
        //    this.budgetRepository.RemoveAsync(budget);
        //}

        //public BudgetModel UpdateBudget(BudgetModel budgetModel)
        //{
        //    Budget budget = mapper.Map<Budget>(budgetModel);
        //    this.budgetRepository.UpdateAsync(budget);

        //    return budgetModel;
        //}

        public async Task RemoveItemAsync(int id)
        {
            BudgetItem item = this.budgetItemRepository.GetAll().FirstOrDefault(i => i.Id == id);
            item.IsDeleted = true;
            await this.budgetItemRepository.RemoveAsync(item);
        }
    }
}
